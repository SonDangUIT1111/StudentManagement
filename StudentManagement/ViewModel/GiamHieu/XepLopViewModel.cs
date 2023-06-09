﻿using StudentManagement.Model;
using StudentManagement.ViewModel.MessageBox;
using StudentManagement.Views.GiamHieu;
using StudentManagement.Views.MessageBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StudentManagement.ViewModel.GiamHieu
{
    public class XepLopViewModel : BaseViewModel
    {

        // khai báo biến
        private Lop _lopHocDangChon;
        public Lop LopHocDangChon { get { return _lopHocDangChon; } set { _lopHocDangChon = value; OnPropertyChanged(); } }
        public XepLopChoHocSinh XepLopWD { get; set; }
        private ObservableCollection<StudentManagement.Model.HocSinh> _danhSachHocSinh;
        public ObservableCollection<StudentManagement.Model.HocSinh> DanhSachHocSinh { get => _danhSachHocSinh; set { _danhSachHocSinh = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _namSinhCmb;
        public ObservableCollection<string> NamSinhCmb { get => _namSinhCmb; set { _namSinhCmb = value; OnPropertyChanged(); } }
        private bool[] _selectCheckBox;
        public bool[] SelectCheckBox { get => _selectCheckBox; set { _selectCheckBox = value; OnPropertyChanged(); } }

        private bool _progressBarVisibility;
        public bool ProgressBarVisibility
        {
            get
            {
                return _progressBarVisibility;
            }
            set
            {
                _progressBarVisibility = value;
                OnPropertyChanged();
            }
        }

        // khai báo ICommand
        public ICommand FindTheoNamSinh { get; set; }
        public ICommand Filter { get; set; }
        public ICommand DanhDau { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand XepLop { get; set; }
        public ICommand LoadWindow { get; set; }
        public XepLopViewModel()
        {
            LopHocDangChon = new Lop();
            LoadNamSinh();
            // define command
            LoadWindow = new RelayCommand<XepLopChoHocSinh>((parameter) => { return true; }, async (parameter) =>
            {
                XepLopWD = parameter;
                ProgressBarVisibility = true;
                await LoadDanhSachHocSinh();
                ProgressBarVisibility = false;
                SelectCheckBox = new bool[DanhSachHocSinh.Count];
            });
            FindTheoNamSinh = new RelayCommand<object>((parameter) => { return true; }, async (parameter) =>
             {
                 ComboBox cmb = parameter as ComboBox;
                 string value = cmb.SelectedItem as string;
                 ProgressBarVisibility = true;
                 await LoadDanhSachTheoNamSinh(value);
                 ProgressBarVisibility = false;
                 ClearSelectArray();
             });
            Filter = new RelayCommand<object>((parameter) => { return true; }, async (parameter) =>
             {
                 TextBox textBox = parameter as TextBox;
                 string text = textBox.Text;
                 ProgressBarVisibility = true;
                 await LocDanhSach(text);
                 ClearSelectArray();
                 ProgressBarVisibility = false;
             });
            DanhDau = new RelayCommand<DataGrid>((parameter) => { return true; }, (parameter) =>
            {
                int location = parameter.SelectedIndex;
                SelectCheckBox[location] = !SelectCheckBox[location];
            });
            CancelCommand = new RelayCommand<object>((parameter) => { return true; }, (parameter) =>
            {
                XepLopWD.Close();
            });
            XepLop = new RelayCommand<DataGrid>((parameter) => { return true; }, (parameter) =>
            {
                ThemHocSinhVaoLop();
            });
        }
        void LoadNamSinh()
        {
            NamSinhCmb = new ObservableCollection<string>();
            using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
            {
                try
                {
                    try 
                    { 
                        con.Open(); 
                    } catch (Exception) 
                    {
                        MessageBoxFail messageBoxFail = new MessageBoxFail();
                        messageBoxFail.ShowDialog(); 
                        return; 
                    }
                    string CmdString = "select distinct Year(NgaySinh) from HocSinh where TenHocSinh is not null and (MaLop <> " + LopHocDangChon.MaLop.ToString()
                                   + " or MaLop is null)";
                    SqlCommand cmd = new SqlCommand(CmdString, con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            NamSinhCmb.Add(reader.GetInt32(0).ToString());
                        }
                        reader.NextResult();
                    }
                    con.Close();

                }
                catch (Exception)
                {
                    MessageBoxFail msgBoxFail = new MessageBoxFail();
                    msgBoxFail.ShowDialog();
                    return;
                }
            }
        }
        public async Task LoadDanhSachHocSinh()
        {
            DanhSachHocSinh = new ObservableCollection<Model.HocSinh>();
            using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
            {
                try
                {
                    try
                    {
                        await con.OpenAsync();
                    }
                    catch (Exception)
                    {
                        MessageBoxFail messageBoxFail = new MessageBoxFail();
                        messageBoxFail.ShowDialog();
                        return;
                    }
                    string CmdString = "select * from HocSinh where TenHocSinh is not null and (MaLop <> " + LopHocDangChon.MaLop.ToString()
                                    + " or MaLop is null)";
                    SqlCommand cmd = new SqlCommand(CmdString, con);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    while (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            StudentManagement.Model.HocSinh student = new StudentManagement.Model.HocSinh
                            {
                                MaHocSinh = reader.GetInt32(0),
                                TenHocSinh = reader.GetString(1),
                                NgaySinh = reader.GetDateTime(2),
                                GioiTinh = reader.GetBoolean(3),
                                DiaChi = reader.GetString(4),
                                Email = reader.GetString(5),
                                Avatar = (byte[])reader[6],
                            };
                            DanhSachHocSinh.Add(student);
                        }
                        await reader.NextResultAsync();
                    }
                    con.Close();
                }
                catch (Exception)
                {
                    MessageBoxFail messageBoxFail = new MessageBoxFail();
                    messageBoxFail.ShowDialog();
                    return;
                }
            }
        }


        public async Task LoadDanhSachTheoNamSinh(string value)
        {
            DanhSachHocSinh.Clear();
            using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
            {
                try
                {
                    try
                    {
                        await con.OpenAsync();
                    }
                    catch (Exception)
                    {
                        MessageBoxFail messageBoxFail = new MessageBoxFail();
                        messageBoxFail.ShowDialog();
                        return;
                    }
                    string CmdString = "select * from HocSinh where TenHocSinh is not null and Year(NgaySinh) = " + value + " and (MaLop <> " + LopHocDangChon.MaLop.ToString()
                                    + " or MaLop is null)" + " and TenHocSinh like '%" + XepLopWD.tbSearch.Text + "%'";
                    SqlCommand cmd = new SqlCommand(CmdString, con);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    while (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            StudentManagement.Model.HocSinh student = new StudentManagement.Model.HocSinh
                            {
                                MaHocSinh = reader.GetInt32(0),
                                TenHocSinh = reader.GetString(1),
                                NgaySinh = reader.GetDateTime(2),
                                GioiTinh = reader.GetBoolean(3),
                                DiaChi = reader.GetString(4),
                                Email = reader.GetString(5),
                                Avatar = (byte[])reader[6]
                            };
                            DanhSachHocSinh.Add(student);
                        }
                        await reader.NextResultAsync();
                    }
                    con.Close();
                }
                catch (Exception)
                {
                    MessageBoxFail messageBoxFail = new MessageBoxFail();
                    messageBoxFail.ShowDialog();
                    return;
                }
            }
        }
        public async Task LocDanhSach(string value)
        {
            DanhSachHocSinh.Clear();
            using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
            {
                try
                {
                    try
                    {
                        await con.OpenAsync();
                    }
                    catch (Exception)
                    {
                        MessageBoxFail messageBoxFail = new MessageBoxFail();
                        messageBoxFail.ShowDialog();
                        return;
                    }
                    string CmdString = "select * from HocSinh where TenHocSinh is not null and TenHocSinh like N'%" + value + "%' and (MaLop <>" + LopHocDangChon.MaLop.ToString()
                                    + " or MaLop is null)";

                    if (XepLopWD.cmbNamSinh.SelectedItem != null)
                    {
                        CmdString = "select * from HocSinh where TenHocSinh is not null and TenHocSinh like N'%" + value + "%' and (MaLop <>" + LopHocDangChon.MaLop.ToString()
                                        + " or MaLop is null) and Year(NgaySinh) = " + XepLopWD.cmbNamSinh.SelectedItem.ToString();
                    }
                    SqlCommand cmd = new SqlCommand(CmdString, con);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    while (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            StudentManagement.Model.HocSinh student = new StudentManagement.Model.HocSinh
                            {
                                MaHocSinh = reader.GetInt32(0),
                                TenHocSinh = reader.GetString(1),
                                NgaySinh = reader.GetDateTime(2),
                                GioiTinh = reader.GetBoolean(3),
                                DiaChi = reader.GetString(4),
                                Email = reader.GetString(5),
                                Avatar = (byte[])reader[6]
                            };
                            DanhSachHocSinh.Add(student);
                        }
                        await reader.NextResultAsync();
                    }
                    con.Close();
                }
                catch (Exception)
                {
                    MessageBoxFail messageBoxFail = new MessageBoxFail();
                    messageBoxFail.ShowDialog();
                    return;
                }
            }
        }

        public void ThemHocSinhVaoLop()
        {
            MessageBoxYesNo wd = new MessageBoxYesNo();

            var data = wd.DataContext as MessageBoxYesNoViewModel;
            data.Title = "Xác nhận!";
            data.Question = "Bạn có chắc chắn muốn thêm những học sinh này vào lớp "
                                                        + LopHocDangChon.TenLop;
            wd.ShowDialog();

            var result = wd.DataContext as MessageBoxYesNoViewModel;

            if (result.IsYes == true)
            {
                using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
                {
                    try
                    {
                        try
                        { 
                            con.Open(); 
                        } catch (Exception) 
                        {
                            MessageBoxFail messageBoxFail = new MessageBoxFail();
                            messageBoxFail.ShowDialog();
                            return;
                        }
                        int numberOfStudents = 0;
                        for (int i = 0; i<SelectCheckBox.Length; i++)
                        {
                            if (SelectCheckBox[i] == true)
                            {
                                numberOfStudents++;
                            }
                        }    
                        List<int> quiDinh = new List<int>();
                        string cmdTest = "select GiaTri,SiSo from QuiDinh,Lop where MaQuiDinh = 1 and MaLop = "+LopHocDangChon.MaLop;
                        SqlCommand cmd1 = new SqlCommand(cmdTest, con);
                        SqlDataReader readerTest = cmd1.ExecuteReader();
                        readerTest.Read();
                        quiDinh.Add(readerTest.GetInt32(0));
                        quiDinh.Add(readerTest.GetInt32(1));
                        if (numberOfStudents+quiDinh[1] > quiDinh[0])
                        {
                            MessageBoxOK messageBoxOK = new MessageBoxOK();
                            MessageBoxOKViewModel datamb = messageBoxOK.DataContext as MessageBoxOKViewModel;
                            datamb.Content = "Bị vượt quá sĩ số tối đa của 1 lớp, mỗi lớp chỉ có tối đa " + quiDinh[0].ToString() + " học sinh";
                            messageBoxOK.ShowDialog();
                            return;
                        }
                        readerTest.Close();

                        string CmdString = "";
                        SqlCommand cmd;
                        for (int i = 0; i < SelectCheckBox.Length; i++)
                        {
                            if (SelectCheckBox[i] == true)
                            {
                                CmdString = "Update HocSinh set MaLop = " + LopHocDangChon.MaLop  +
                                            " where MaHocSinh = " + DanhSachHocSinh[i].MaHocSinh;
                                cmd = new SqlCommand(CmdString, con);
                                cmd.ExecuteScalar();

                                CmdString = "Update HeThongDiem set MaLop = " + LopHocDangChon.MaLop +
                                            " where MaHocSinh = " + DanhSachHocSinh[i].MaHocSinh;
                                cmd = new SqlCommand(CmdString, con);
                                cmd.ExecuteScalar();

                                CmdString = "Update ThanhTich set MaLop = " + LopHocDangChon.MaLop +
                                            " where MaHocSinh = " + DanhSachHocSinh[i].MaHocSinh;
                                cmd = new SqlCommand(CmdString, con);
                                cmd.ExecuteScalar();
                            }
                        }
                        con.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBoxOK messageBoxOK = new MessageBoxOK();
                        MessageBoxOKViewModel datamb = messageBoxOK.DataContext as MessageBoxOKViewModel;
                        datamb.Content = ex.Message;
                        messageBoxOK.ShowDialog();
                        return;
                        return;
                    }
                }
            }
            MessageBoxSuccessful messageBoxSuccessful = new MessageBoxSuccessful();
            messageBoxSuccessful.ShowDialog();
            XepLopWD.Close();

        }
        public void ClearSelectArray()
        {
            for (int i = 0; i < SelectCheckBox.Length; i++)
            {
                SelectCheckBox[i] = false;
            }
        }
    }
}
