﻿using MaterialDesignThemes.Wpf;
using StudentManagement.Model;
using StudentManagement.Views.GiamHieu;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StudentManagement.ViewModel.GiamHieu
{
    public class DanhSachLopViewModel:BaseViewModel
    {
        private int _maLop;
        public int MaLop { get { return _maLop; } set { _maLop = value;} }
        private string _tenLop;
        public string TenLop { get { return _tenLop; } set { _tenLop = value;} }
        public DanhSachLop DanhSachLopWindow { get; set; }
        private ObservableCollection<StudentManagement.Model.HocSinh> _danhSachLop;
        public ObservableCollection<StudentManagement.Model.HocSinh> DanhSachLop { get => _danhSachLop; set { _danhSachLop = value;OnPropertyChanged(); } }
        

        // declare ICommand

        public ICommand ThemHocSinh { get; set; }
        public ICommand RemoveKhoiLop { get; set; }
        public ICommand LoadWindow { get; set; }
        public DanhSachLopViewModel()
        {
            MaLop = 100;
            TenLop = "10A1";
            LoadDanhSachHocSinh();
            LoadWindow = new RelayCommand<DanhSachLop>((parameter) => { return true; }, (parameter) =>
            {
                DanhSachLopWindow = parameter;
            });
            ThemHocSinh = new RelayCommand<object>((parameter) => { return true; }, (parameter) =>
            {
                XepLopChoHocSinh window = new XepLopChoHocSinh();
                XepLopViewModel data = window.DataContext as XepLopViewModel;
                data.LopHocDangChon.MaLop = MaLop;
                data.LopHocDangChon.TenLop = TenLop;
                window.ShowDialog();
                LoadDanhSachHocSinh();
            });
            RemoveKhoiLop = new RelayCommand<Model.HocSinh>((parameter) => { return true; }, (parameter) =>
            {
                Model.HocSinh item = parameter;
                XoaHocSinh(item);
                LoadDanhSachHocSinh();
                // Hiện snackbar thông báo xóa thành công, có thể hoàn tác
                DanhSachLopWindow.Snackbar.MessageQueue?.Enqueue(
                $"Xóa thành công",
                $"Hoàn tác",
                param => { HoanTac(item); },
                TimeSpan.FromSeconds(5));
            });
        }
        public void LoadDanhSachHocSinh()
        {
            DanhSachLop = new ObservableCollection<Model.HocSinh>();
            using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
            {
                con.Open();
                string CmdString = "select * from HocSinh where TenHocSinh is not null and MaLop = "+MaLop.ToString();
                SqlCommand cmd = new SqlCommand(CmdString, con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        StudentManagement.Model.HocSinh student = new StudentManagement.Model.HocSinh
                        {
                            MaHocSinh = reader.GetInt32(0),
                            TenHocSinh = reader.GetString(1),
                            NgaySinh = reader.GetDateTime(2),
                            GioiTinh = reader.GetBoolean(3),
                            DiaChi = reader.GetString(4),
                            Email = reader.GetString(5),
                        };
                        DanhSachLop.Add(student);
                    }
                    reader.NextResult();
                }
                con.Close();
            }
        }
        public void XoaHocSinh(Model.HocSinh value)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
            {
                con.Open();
                SqlCommand cmd;
                string CmdString = "Update HocSinh set MaLop = null, TenLop = null where MaHocSinh = "+value.MaHocSinh;
                cmd = new SqlCommand(CmdString, con);
                cmd.ExecuteScalar();

                CmdString = "Update HeThongDiem set MaLop = null, TenLop = null where MaHocSinh = " + value.MaHocSinh;
                cmd = new SqlCommand(CmdString, con);
                cmd.ExecuteScalar();

                CmdString = "Update ThanhTich set MaLop = null, TenLop = null where MaHocSinh = " + value.MaHocSinh;
                cmd = new SqlCommand(CmdString, con);
                cmd.ExecuteScalar();
                con.Close();
            }
        }
        public void HoanTac(Model.HocSinh value)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
            {
                con.Open();
                SqlCommand cmd;
                string CmdString = "Update HocSinh set MaLop = "+MaLop.ToString()+", TenLop = '"+TenLop+"' where MaHocSinh = " + value.MaHocSinh;
                cmd = new SqlCommand(CmdString, con);
                cmd.ExecuteScalar();

                CmdString = "Update HeThongDiem set MaLop = " + MaLop.ToString() + ", TenLop = '" + TenLop + "' where MaHocSinh = " + value.MaHocSinh;
                cmd = new SqlCommand(CmdString, con);
                cmd.ExecuteScalar();

                CmdString = "Update ThanhTich set MaLop = " + MaLop.ToString() + ", TenLop = '" + TenLop + "' where MaHocSinh = " + value.MaHocSinh;
                cmd = new SqlCommand(CmdString, con);
                cmd.ExecuteScalar();
                con.Close();
            }
            LoadDanhSachHocSinh();
        }

    }
}
