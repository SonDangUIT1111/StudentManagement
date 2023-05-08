﻿using Microsoft.Win32;
using StudentManagement.Converter;
using StudentManagement.Model;
using StudentManagement.Views.GiamHieu;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StudentManagement.ViewModel.GiamHieu
{
    public class SuaThongTinHocSinhViewModel : BaseViewModel
    {
        public SuaThongTinHocSinh SuaThongTinHocSinhWD { get; set; }
        public string ImagePath { get; set; }
        private StudentManagement.Model.HocSinh _hocSinhHienTai;
        public StudentManagement.Model.HocSinh HocSinhHienTai { get => _hocSinhHienTai; set { _hocSinhHienTai = value; OnPropertyChanged(); } }
        public ICommand LoadData { get; set; }
        public ICommand ConfirmChange { get; set; }
        public ICommand ChangeImage { get; set; }
        public ICommand CancelChange { get; set; }
        public SuaThongTinHocSinhViewModel()
        {
            LoadData = new RelayCommand<SuaThongTinHocSinh>((parameter) => { return true; }, (parameter) =>
            {
                SuaThongTinHocSinhWD = parameter;
                //MessageBox.Show(HocSinhHienTai.GioiTinh.ToString());
                if (HocSinhHienTai.GioiTinh.ToString() == "True")
                {
                    SuaThongTinHocSinhWD.Male.IsChecked = true;
                }
                else
                {
                    SuaThongTinHocSinhWD.Female.IsChecked = true;
                }
            });
            CancelChange = new RelayCommand<object>((parameter) => { return true; }, (parameter) =>
            {
                SuaThongTinHocSinhWD.Close();

            });
            ChangeImage = new RelayCommand<Grid>((parameter) => { return true; }, (parameter) =>
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Title = "Insert Image";
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" + "JPEG (.jpg;.jpeg)|*.jpg;*.jpeg|" + "Portable Network Graphic (.png)|.png";
                if (op.ShowDialog() == true)
                {
                    ImagePath = op.FileName;
                    try
                    {
                        ImageBrush imageBrush = new ImageBrush();
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.UriSource = new Uri(ImagePath);
                        bitmap.EndInit();
                        imageBrush.ImageSource = bitmap;
                        parameter.Background = imageBrush;
                        if (parameter.Children.Count > 1)
                        {
                            parameter.Children.Remove(parameter.Children[0]);
                            parameter.Children.Remove(parameter.Children[1]);
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Lỗi, không cập nhật được hình ảnh.");
                    }
                }
            });
            ConfirmChange = new RelayCommand<object>((parameter) => { return true; }, (parameter) =>
            {
                if (String.IsNullOrEmpty(SuaThongTinHocSinhWD.HoTen.Text) || String.IsNullOrEmpty(SuaThongTinHocSinhWD.NgaySinh.SelectedDate.Value.ToString()) ||
                    String.IsNullOrEmpty(SuaThongTinHocSinhWD.DiaChi.Text) || String.IsNullOrEmpty(SuaThongTinHocSinhWD.Email.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                }
                else if (!Regex.IsMatch(SuaThongTinHocSinhWD.Email.Text, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
                {
                    MessageBox.Show("Email không hợp lệ, vui lòng nhập lại!");
                }
                else
                {
                    MessageBoxResult ConfirmAdd = System.Windows.MessageBox.Show("Bạn có muốn sửa thông tin học sinh này không?", "Change Confirmation", System.Windows.MessageBoxButton.YesNo);
                    if (ConfirmAdd == MessageBoxResult.Yes)
                    {
                        using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
                        {
                            try
                            {
                                try { con.Open(); } catch (Exception) { MessageBox.Show("Lỗi mạng, vui lòng kiểm tra lại đường truyền"); return; }
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Lỗi mạng, vui lòng kiểm tra đường truyền");
                            }
                            string CmdString = @"update HocSinh set TenHocSinh = N'" + SuaThongTinHocSinhWD.HoTen.Text + "', NgaySinh = '" + SuaThongTinHocSinhWD.NgaySinh.SelectedDate.Value.Year + '-' + SuaThongTinHocSinhWD.NgaySinh.SelectedDate.Value.Month + '-' + SuaThongTinHocSinhWD.NgaySinh.SelectedDate.Value.Day + "', GioiTinh = ";
                            if (SuaThongTinHocSinhWD.Male.IsChecked == true)
                            {
                                CmdString += "1, ";
                            }
                            else
                            {
                                CmdString += "0, ";
                            }
                            CmdString = CmdString + "DiaChi = N'" + SuaThongTinHocSinhWD.DiaChi.Text + "', Email = '" + SuaThongTinHocSinhWD.Email.Text + "', AnhThe = @imagebinary where MaHocSinh = " + HocSinhHienTai.MaHocSinh;
                            //MessageBox.Show(CmdString);
                            ByteArrayToBitmapImageConverter converter = new ByteArrayToBitmapImageConverter();
                            byte[] buffer = converter.ImageToBinary(ImagePath);
                            // Định nghĩa @imagebinary
                            SqlCommand cmd = new SqlCommand(CmdString, con);
                            SqlParameter sqlParam = cmd.Parameters.AddWithValue("@imagebinary", buffer);
                            sqlParam.DbType = DbType.Binary;
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                        MessageBox.Show("Sửa thông tin học sinh thành công!");
                        SuaThongTinHocSinhWD.Close();
                    }

                }

            });
        }
    }
}
