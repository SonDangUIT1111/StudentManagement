﻿using StudentManagement.Model;
using StudentManagement.Views.GiamHieu;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace StudentManagement.ViewModel.GiamHieu
{
    public class ThemLopHocViewModel : BaseViewModel
    {
        public ThemLopHoc ThemLopHocWD { get; set; }
        public string GiaoVienQueries { get; set; }
        public int MaGiaoVien { get; set; }
        public int MaKhoi { get; set; }
        public int Khoi { get; set; }
        public ICommand AddClass { get; set; }
        public ICommand LoadData { get; set; }
        public ICommand CancelAddClass { get; set; }

        private ObservableCollection<string> _giaoVienComboBox;
        public ObservableCollection<string> GiaoVienComboBox
        {
            get => _giaoVienComboBox;
            set { _giaoVienComboBox = value; OnPropertyChanged(); }
        }

        private string selectedGiaoVien;

        public string SelectedGiaoVien
        {
            get { return selectedGiaoVien; }
            set
            {
                if (selectedGiaoVien != value)
                {
                    selectedGiaoVien = value;
                    LayMaGiaoVien();
                }
            }
        }

        public void LayMaGiaoVien()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
            {
                try
                {
                    try { con.Open(); } catch (Exception) { MessageBox.Show("Lỗi mạng, vui lòng kiểm tra lại đường truyền"); return; }

                }
                catch (Exception)
                {
                    MessageBox.Show("Lỗi mạng, vui lòng kiểm tra lại đường truyền");
                    return;
                }

                string cmdString = "SELECT MaGiaoVien FROM GiaoVien WHERE TenGiaoVien = '" + selectedGiaoVien + "'";
                SqlCommand cmd = new SqlCommand(cmdString, con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //GiaoVienComboBox.Add(reader.GetString(0));
                        //if (string.IsNullOrEmpty(GiaoVienQueries))
                        //{
                        //    GiaoVienQueries = reader.GetString(0);
                        //}
                        MaGiaoVien = reader.GetInt32(0);
                    }
                    reader.NextResult();
                }
                con.Close();
            }
        }

        public ThemLopHocViewModel()
        {
            LoadGiaoVien();

            LoadData = new RelayCommand<object>((parameter) => { return true; }, (parameter) =>
            {
                ThemLopHocWD = parameter as ThemLopHoc;
            });

            AddClass = new RelayCommand<object>((parameter) => { return true; }, (parameter) =>
            {
                if (String.IsNullOrEmpty(ThemLopHocWD.ClassName.Text) || String.IsNullOrEmpty(ThemLopHocWD.NumberOfStudent.Text) || String.IsNullOrEmpty(ThemLopHocWD.AcademyYear.Text) || String.IsNullOrEmpty(selectedGiaoVien))
                {
                    MessageBox.Show("Nhập đầy đủ thông tin");
                }
                else using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
                    {
                        try
                        {
                            try { con.Open(); } catch (Exception) { MessageBox.Show("Lỗi mạng, vui lòng kiểm tra lại đường truyền"); return; }

                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Lỗi mạng, vui lòng kiểm tra lại đường truyền");
                            return;
                        }



                        string cmdString = "INSERT INTO Lop(TenLop, SiSo, NienKhoa, MaKhoi, Khoi, MaGVCN, TenGVCN) VALUES ('" + ThemLopHocWD.ClassName.Text + "', '" + ThemLopHocWD.NumberOfStudent.Text + "', '" + ThemLopHocWD.AcademyYear.Text + "'";

                        switch (ThemLopHocWD.ClassName.Text.Substring(0, 2))
                        {
                            case "10":
                                MaKhoi = 1;
                                Khoi = 10;
                                break;
                            case "11":
                                MaKhoi = 2;
                                Khoi = 11;
                                break;
                            case "12":
                                MaKhoi = 3;
                                Khoi = 12;
                                break;
                        }

                        cmdString = cmdString + ", '" + MaKhoi + "', '" + Khoi + "','" + MaGiaoVien + "', '" + selectedGiaoVien + "')";
                        SqlCommand cmd = new SqlCommand(cmdString, con);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        MessageBox.Show("Thêm lớp học thành công");
                        ThemLopHocWD.Close();
                    }
            });

            CancelAddClass = new RelayCommand<object>((parameter) => { return true; }, (parameter) =>
            {
                ThemLopHocWD.Close();
            });
        }

        public void LoadGiaoVien()
        {
            GiaoVienComboBox = new ObservableCollection<string>();
            using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
            {
                try
                {
                    try { con.Open(); } catch (Exception) { MessageBox.Show("Lỗi mạng, vui lòng kiểm tra lại đường truyền"); return; }

                }
                catch (Exception)
                {
                    MessageBox.Show("Lỗi mạng, vui lòng kiểm tra lại đường truyền");
                    return;
                };
                string cmdString = "SELECT DISTINCT TenGiaoVien FROM GiaoVien WHERE TenGiaoVien NOT IN (SELECT DISTINCT TenGVCN FROM Lop)";
                SqlCommand cmd = new SqlCommand(cmdString, con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        GiaoVienComboBox.Add(reader.GetString(0));
                        if (string.IsNullOrEmpty(GiaoVienQueries))
                        {
                            GiaoVienQueries = reader.GetString(0);
                        }
                    }
                    reader.NextResult();
                }
                con.Close();
            }
        }
    }
}
