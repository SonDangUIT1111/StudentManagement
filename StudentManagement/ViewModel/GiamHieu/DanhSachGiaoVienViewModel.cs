﻿using StudentManagement.Model;
using StudentManagement.Views.GiamHieu;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StudentManagement.ViewModel.GiamHieu
{
    public class DanhSachGiaoVienViewModel : BaseViewModel
    {
        private ObservableCollection<StudentManagement.Model.GiaoVien> _danhSachGiaoVien;
        public ObservableCollection<StudentManagement.Model.GiaoVien> DanhSachGiaoVien { get => _danhSachGiaoVien; set { _danhSachGiaoVien = value; OnPropertyChanged(); } }

        //declare ICommand
        public ICommand LocGiaoVien { get; set; }
        public ICommand ThemGiaoVien { get; set; }
        public ICommand UpdateGiaoVien { get; set; }
        public ICommand RemoveGiaoVien { get; set; }

        public DanhSachGiaoVienViewModel()
        {
            LoadDanhSachGiaoVien();
            LocGiaoVien = new RelayCommand<TextBox>((parameter) => { return true; }, (parameter) =>
            {
                TextBox tb = parameter;
                LocGiaoVienTheoTen(tb.Text);
            });
            ThemGiaoVien = new RelayCommand<object>((parameter) => { return true; }, (parameter) =>
            {
                ThemGiaoVien window = new ThemGiaoVien();
                ThemGiaoVienViewModel data = window.DataContext as ThemGiaoVienViewModel;

                window.ShowDialog();
            });
            UpdateGiaoVien = new RelayCommand<Model.GiaoVien>((parameter) => { return true; }, (parameter) =>
            {
                SuaGiaoVien window = new SuaGiaoVien();
                SuaGiaoVienViewModel data = window.DataContext as SuaGiaoVienViewModel;
                data.GiaoVienHienTai = parameter;
                window.ShowDialog();
                LoadDanhSachGiaoVien();
            });
            RemoveGiaoVien = new RelayCommand<Model.GiaoVien>((parameter) => { return true; }, (parameter) =>
            {
                Model.GiaoVien item = parameter;
                XoaGiaoVien(item);
                LoadDanhSachGiaoVien();
            });
        }
        public void LoadDanhSachGiaoVien()
        {
            DanhSachGiaoVien = new ObservableCollection<Model.GiaoVien>();
            using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
            {
                try
                {
                    try { con.Open(); } catch (Exception) { MessageBox.Show("Lỗi mạng, vui lòng kiểm tra lại đường truyền"); return; }
                    string CmdString = "select MaGiaoVien,TenGiaoVien,NgaySinh,GioiTinh,DiaChi,Email,AnhThe from GiaoVien";
                    SqlCommand cmd = new SqlCommand(CmdString, con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            StudentManagement.Model.GiaoVien teacher = new StudentManagement.Model.GiaoVien();
                            teacher.MaGiaoVien = reader.GetInt32(0);
                            teacher.TenGiaoVien = reader.GetString(1);
                            teacher.NgaySinh = reader.GetDateTime(2);
                            teacher.GioiTinh = reader.GetBoolean(3);
                            teacher.DiaChi = reader.GetString(4);
                            teacher.Email = reader.GetString(5);
                            teacher.Avatar = (byte[])reader[6];
                            DanhSachGiaoVien.Add(teacher);
                        }
                        reader.NextResult();
                    }
                    con.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("Lỗi mạng, vui lòng kiểm tra lại đường truyền");
                }
            }
        }
        public void LocGiaoVienTheoTen(string value)
        {
            DanhSachGiaoVien.Clear();
            using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
            {
                try
                {
                    try { con.Open(); } catch (Exception) { MessageBox.Show("Lỗi mạng, vui lòng kiểm tra lại đường truyền"); return; }
                    string CmdString = "select * from GiaoVien where TenGiaoVien is not null and TenGiaoVien like '%" + value + "%'";
                    SqlCommand cmd = new SqlCommand(CmdString, con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            StudentManagement.Model.GiaoVien teacher = new StudentManagement.Model.GiaoVien
                            {
                                MaGiaoVien = reader.GetInt32(0),
                                TenGiaoVien = reader.GetString(1),
                                NgaySinh = reader.GetDateTime(2),
                                GioiTinh = reader.GetBoolean(3),
                                DiaChi = reader.GetString(4),
                                Email = reader.GetString(5),
                            };
                            DanhSachGiaoVien.Add(teacher);
                        }
                        reader.NextResult();
                    }
                    con.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("Lỗi mạng, vui lòng kiểm tra lại đường truyền");
                }
            }
        }
        public void XoaGiaoVien(Model.GiaoVien value)
        {
            MessageBoxResult ConfirmDelete = System.Windows.MessageBox.Show("Bạn có chắc chắn xóa giáo viên?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (ConfirmDelete == MessageBoxResult.Yes)
                using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
                {
                    try
                    {
                        try { con.Open(); } catch (Exception) { MessageBox.Show("Lỗi mạng, vui lòng kiểm tra lại đường truyền"); return; }
                        SqlCommand cmd;
                        string CmdString = "Delete From GiaoVien where MaGiaoVien = " + value.MaGiaoVien;
                        cmd = new SqlCommand(CmdString, con);
                        cmd.ExecuteScalar();
                        con.Close();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Lỗi mạng, vui lòng kiểm tra lại đường truyền");
                    }
                    MessageBox.Show("Đã xóa " + value.TenGiaoVien);
                }
        }
    }
}
