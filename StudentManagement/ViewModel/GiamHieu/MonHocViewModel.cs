﻿using StudentManagement.Model;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MonHoc = StudentManagement.Views.GiamHieu.MonHoc;

namespace StudentManagement.ViewModel.GiamHieu
{
    public class MonHocViewModel : BaseViewModel
    {
        public MonHoc MonHocWD { get; set; }

        private ObservableCollection<StudentManagement.Model.MonHoc> _danhSachMonHoc;
        public ObservableCollection<StudentManagement.Model.MonHoc> DanhSachMonHoc { get => _danhSachMonHoc; set { _danhSachMonHoc = value; OnPropertyChanged(); } }
        public MonHoc _gridSeletecdItem;
        public MonHoc GridSelectedItem
        {
            get { return _gridSeletecdItem; }
            set
            {
                _gridSeletecdItem = value;
                OnPropertyChanged();
            }
        }
        public ICommand LoadData { get; set; }
        public ICommand DeleteSubject { get; set; }
        public ICommand EditSubject { get; set; }
        public ICommand SubjectSearch { get; set; }
        public ICommand AddSubject { get; set; }
        public ICommand AddConfirm { get; set; }


        public MonHocViewModel()
        {
            LoadThongTinMonHoc();
            LoadData = new RelayCommand<object>((parameter) => { return true; }, (parameter) =>
            {
                MonHocWD = parameter as MonHoc;
                //MessageBox.Show("Hello");
            });
            DeleteSubject = new RelayCommand<object>((parameter) => { return true; }, (parameter) =>
            {
                if (MessageBox.Show("Bạn có muốn xoá môn học này không?", "Xoá môn học", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    TextBox textBlock = parameter as TextBox;
                    if (textBlock != null)
                    {
                        try
                        {
                            string subjectName = textBlock.Text;
                            using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
                            {
                                try { con.Open(); } catch (Exception) { MessageBox.Show("Lỗi mạng, vui lòng kiểm tra lại đường truyền"); return; }
                                string CmdString = "delete from MonHoc where TenMon=N'" + subjectName + "'";
                                MessageBox.Show(CmdString);
                                SqlCommand cmd = new SqlCommand(CmdString, con);
                                cmd.ExecuteNonQuery();
                                con.Close();
                            }
                            MessageBox.Show("Thay đổi thành công!");
                            LoadThongTinMonHoc();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Đang có giáo viên được phân công giảng dạy môn này. Vui lòng đảm bảo môn không được giảng dạy mới có thể xóa được.");
                        }
                    }
                }
            });
            EditSubject = new RelayCommand<object>((parameter) => { return true; }, (parameter) =>
            {
                TextBox textBox = parameter as TextBox;
                if (textBox != null )
                {
                    MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn đổi tên", "Thông báo", MessageBoxButton.YesNo); 
                    if (result == MessageBoxResult.Yes)
                    {

                        LoadThongTinMonHoc();
                        MonHocWD.Snackbar.MessageQueue?.Enqueue(
                        $"Đổi tên thành công",
                        $"Hoàn tác",
                        param => {/* HoanTac(item);*/ },
                        TimeSpan.FromSeconds(5));
                    }    
                }
            });
            SubjectSearch = new RelayCommand<TextBox>((parameter) => { return true; }, (parameter) =>
            {
                DanhSachMonHoc.Clear();
                using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
                {
                    try
                    {
                        try { con.Open(); } catch (Exception) { MessageBox.Show("Lỗi mạng, vui lòng kiểm tra lại đường truyền"); return; }
                        string CmdString = "select * from MonHoc where TenMon like '%" + parameter.Text + "%'";
                        SqlCommand cmd = new SqlCommand(CmdString, con);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                StudentManagement.Model.MonHoc monhoc = new StudentManagement.Model.MonHoc();
                                monhoc.MaMon = reader.GetInt32(0);
                                monhoc.TenMon = reader.GetString(1);
                                DanhSachMonHoc.Add(monhoc);
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
            });
            AddSubject = new RelayCommand<TextBox>((parameter) => { return true; }, (parameter) =>
            {
                MonHocWD.txtTenMH.IsEnabled = true;
                MonHocWD.btnThemMonHoc.Visibility = Visibility.Hidden;
                MonHocWD.btnXacNhan.Visibility = Visibility.Visible;
                MonHocWD.btnXacNhan.IsEnabled = true;
            });
            AddConfirm = new RelayCommand<TextBox>((parameter) => { return true; }, (parameter) =>
            {
                string monhoc = parameter.Text;
                if (string.IsNullOrEmpty(monhoc))
                {
                    MessageBox.Show("Vui lòng nhập tên môn học!");
                    return;
                }
                else
                {
                    MessageBox.Show(monhoc);
                    if (MessageBox.Show("Bạn có muốn thêm môn học này không?", "Thêm môn học", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
                        {
                            try
                            {
                                try { con.Open(); } catch (Exception) { MessageBox.Show("Lỗi mạng, vui lòng kiểm tra lại đường truyền"); return; }
                                string CmdString = "insert into MonHoc (TenMon, MaTruong) values (N'" + monhoc + "', 1)";
                                MessageBox.Show(CmdString);
                                SqlCommand cmd = new SqlCommand(CmdString, con);
                                cmd.ExecuteNonQuery();
                                con.Close();
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Lỗi mạng, vui lòng kiểm tra lại đường truyền");
                            }
                        }
                        MessageBox.Show("Thêm môn học thành công!");
                        MonHocWD.txtTenMH.Text = "";
                        MonHocWD.txtTenMH.IsEnabled = false;
                        MonHocWD.btnThemMonHoc.Visibility = Visibility.Visible;
                        MonHocWD.btnXacNhan.Visibility = Visibility.Hidden;
                        MonHocWD.btnXacNhan.IsEnabled = false;
                        LoadThongTinMonHoc();
                    }
                }
            });
        }
        public void LoadThongTinMonHoc()
        {
            DanhSachMonHoc = new ObservableCollection<Model.MonHoc>();
            using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
            {
                try
                {
                    try { con.Open(); } catch (Exception) { MessageBox.Show("Lỗi mạng, vui lòng kiểm tra lại đường truyền"); return; }
                    string CmdString = "select * from MonHoc";
                    SqlCommand cmd = new SqlCommand(CmdString, con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            StudentManagement.Model.MonHoc monhoc = new StudentManagement.Model.MonHoc();
                            monhoc.MaMon = reader.GetInt32(0);
                            monhoc.TenMon = reader.GetString(1);
                            DanhSachMonHoc.Add(monhoc);
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
    }
}
