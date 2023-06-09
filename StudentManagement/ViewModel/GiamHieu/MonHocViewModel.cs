﻿using StudentManagement.Model;
using StudentManagement.ViewModel.MessageBox;
using StudentManagement.Views.MessageBox;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MonHoc = StudentManagement.Views.GiamHieu.MonHoc;

namespace StudentManagement.ViewModel.GiamHieu
{
    public class MonHocViewModel : BaseViewModel
    {
        public bool everLoaded { get; set; }
        public MonHoc MonHocWD { get; set; }
        public bool IsLoadAll { get; set; } = false;

        private ObservableCollection<StudentManagement.Model.MonHoc> _danhSachMonHoc;
        public ObservableCollection<StudentManagement.Model.MonHoc> DanhSachMonHoc { get => _danhSachMonHoc; set { _danhSachMonHoc = value; OnPropertyChanged(); } }

        private Model.MonHoc _monHocEditting;
        public Model.MonHoc MonHocEditting { get => _monHocEditting; set { _monHocEditting = value; OnPropertyChanged(); } }

        private bool _dataGridVisibility;

        public bool DataGridVisibility
        {
            get
            {
                return _dataGridVisibility;
            }
            set
            {
                _dataGridVisibility = value;
                OnPropertyChanged();
            }
        }

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

        public ICommand LoadData { get; set; }
        public ICommand DeleteSubject { get; set; }
        public ICommand EditSubject { get; set; }
        public ICommand SubjectSearch { get; set; }
        public ICommand AddSubject { get; set; }
        public ICommand AddConfirm { get; set; }
        public ICommand SubjectSearchAll { get; set; }

        public ICommand EditEnable { get; set; }
        public ICommand LostFocusTxt { get; set; }


        public MonHocViewModel()
        {
            everLoaded = false;
            MonHocEditting = new Model.MonHoc();
            LoadData = new RelayCommand<object>((parameter) => { return true; }, async (parameter) =>
            {
                if (everLoaded == false)
                {
                    MonHocWD = parameter as MonHoc;
                    DataGridVisibility = false;
                    ProgressBarVisibility = true;
                    await LoadThongTinMonHoc();
                    DataGridVisibility = true;
                    ProgressBarVisibility = false;
                    everLoaded = true;
                }
            });
            DeleteSubject = new RelayCommand<object>((parameter) => { return true; }, async (parameter) =>
            {
                MessageBoxYesNo wd = new MessageBoxYesNo();

                var data = wd.DataContext as MessageBoxYesNoViewModel;
                data.Title = "Xác nhận!";
                data.Question = "Bạn có muốn xóa môn học này không?";
                wd.ShowDialog();

                var result = wd.DataContext as MessageBoxYesNoViewModel;
                if (result.IsYes == true)
                {
                    Model.MonHoc mh = parameter as Model.MonHoc;
                    if (mh != null)
                    {
                        try
                        {
                            using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
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
                                string CmdString = "update MonHoc set ApDung = 0 where MaMon = " + mh.MaMon.ToString();
                                SqlCommand cmd = new SqlCommand(CmdString, con);
                                cmd.ExecuteNonQuery();
                                con.Close();
                            }
                            MessageBoxOK MB = new MessageBoxOK();
                            var datamb = MB.DataContext as MessageBoxOKViewModel;
                            datamb.Content = "Môn học này sẽ không được áp dụng trong dạy học nữa";
                            MB.ShowDialog();
                            DataGridVisibility = false;
                            ProgressBarVisibility = true;
                            await LoadThongTinMonHoc();
                            DataGridVisibility = true;
                            ProgressBarVisibility = false;
                        }
                        catch (Exception)
                        {
                            MessageBoxFail messageBoxFail = new MessageBoxFail();
                            messageBoxFail.ShowDialog();
                        }
                    }
                }
            });
            EditEnable = new RelayCommand<object>((parameter) => { return true; },  (parameter) =>
            {
                MonHocEditting = parameter as Model.MonHoc;
                MonHocWD.txtTenMH.Text = "";
                MonHocWD.txtTenMH.IsEnabled = false;
                MonHocWD.txtTenMH.IsEnabled = true;
                MonHocWD.btnThemMonHoc.Visibility = Visibility.Hidden;
                MonHocWD.btnThemMonHoc.Visibility = Visibility.Hidden;
                MonHocWD.btnXacNhanDoiTen.Visibility = Visibility.Visible;
            });
            EditSubject = new RelayCommand<object>((parameter) => { return true; }, async (parameter) =>
            {
                if (MonHocEditting != null )
                {
                    MessageBoxYesNo wd = new MessageBoxYesNo();

                    var data = wd.DataContext as MessageBoxYesNoViewModel;
                    data.Title = "Xác nhận!";
                    data.Question = "Bạn có chắc chắn muốn đổi tên";
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
                                string CmdString1 = "select * from MonHoc where TenMon = N'" + MonHocWD.txtTenMH.Text + "'";
                                SqlCommand cmd1 = new SqlCommand(CmdString1, con);
                                int count = Convert.ToInt32(cmd1.ExecuteScalar());
                                if (count > 0)
                                {
                                    MessageBoxOK MB = new MessageBoxOK();
                                    var datamb = MB.DataContext as MessageBoxOKViewModel;
                                    datamb.Content = "Tên môn học đã tồn tại, vui lòng chọn tên khác ";
                                    MB.ShowDialog();
                                    con.Close();
                                    MonHocWD.txtTenMH.Text = "";
                                    MonHocWD.txtTenMH.IsEnabled = false;
                                    MonHocWD.btnThemMonHoc.Visibility = Visibility.Visible;
                                    MonHocWD.btnXacNhan.Visibility = Visibility.Hidden;
                                    MonHocWD.btnXacNhanDoiTen.Visibility = Visibility.Hidden;
                                    return;
                                }

                                string CmdString = "update MonHoc set TenMon = N'"+ MonHocWD.txtTenMH.Text + "' where MaMon = "+MonHocEditting.MaMon.ToString();
                                SqlCommand cmd = new SqlCommand(CmdString, con);
                                cmd.ExecuteNonQuery();
                                con.Close();
                                MessageBoxSuccessful messageBoxSuccessful = new MessageBoxSuccessful();
                                messageBoxSuccessful.ShowDialog();
                                MonHocWD.txtTenMH.Text = "";
                                MonHocWD.txtTenMH.IsEnabled = false;
                                MonHocWD.btnThemMonHoc.Visibility = Visibility.Visible;
                                MonHocWD.btnXacNhan.Visibility = Visibility.Hidden;
                                MonHocWD.btnXacNhanDoiTen.Visibility = Visibility.Hidden;
                            }
                            catch (Exception)
                            {
                                MessageBoxFail messageBoxFail = new MessageBoxFail();
                                messageBoxFail.ShowDialog();
                            }
                        }
                        DataGridVisibility = false;
                        ProgressBarVisibility = true;
                        await LoadThongTinMonHoc();
                        DataGridVisibility = true;
                        ProgressBarVisibility = false;
                    }    
                }
            });
            SubjectSearch = new RelayCommand<TextBox>((parameter) => { return true; },async (parameter) =>
            {
                DanhSachMonHoc.Clear();
                ProgressBarVisibility = true;
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
                        string CmdString = "select * from MonHoc where TenMon like N'%" + parameter.Text + "%' and ApDung = 1";
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
                        MessageBoxFail messageBoxFail = new MessageBoxFail();
                        messageBoxFail.ShowDialog();
                    }
                    ProgressBarVisibility = false;
                }
            });
            AddSubject = new RelayCommand<TextBox>((parameter) => { return true; }, (parameter) =>
            {
                MonHocWD.txtTenMH.IsEnabled = true;
                MonHocWD.btnThemMonHoc.Visibility = Visibility.Hidden;
                MonHocWD.btnXacNhan.Visibility = Visibility.Visible;
                MonHocWD.btnXacNhanDoiTen.Visibility = Visibility.Hidden;
            });
            AddConfirm = new RelayCommand<TextBox>((parameter) => { return true; },async (parameter) =>
            {
                string monhoc = parameter.Text;
                if (string.IsNullOrEmpty(monhoc))
                {
                    MessageBoxOK MB = new MessageBoxOK();
                    var datamb = MB.DataContext as MessageBoxOKViewModel;
                    datamb.Content = "Vui lòng nhập tên môn học.";
                    MB.ShowDialog();
                    return;
                }
                else
                {
                    MessageBoxYesNo wd = new MessageBoxYesNo();

                    var data = wd.DataContext as MessageBoxYesNoViewModel;
                    data.Title = "Xác nhận!";
                    data.Question = "Bạn có muốn thêm môn học này không?";
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
                                string CmdString = "select * from MonHoc where TenMon = N'" + monhoc+"'";
                                SqlCommand cmd = new SqlCommand(CmdString, con);
                                int count = Convert.ToInt32(cmd.ExecuteScalar());   
                                if (count > 0 )
                                {
                                    MessageBoxOK MB = new MessageBoxOK();
                                    var datamb = MB.DataContext as MessageBoxOKViewModel;
                                    datamb.Content = "Tên môn học đã tồn tại, vui lòng chọn tên khác";
                                    MB.ShowDialog();
                                    con.Close();
                                    MonHocWD.txtTenMH.Text = "";
                                    MonHocWD.txtTenMH.IsEnabled = false;
                                    MonHocWD.btnThemMonHoc.Visibility = Visibility.Visible;
                                    MonHocWD.btnXacNhan.Visibility = Visibility.Hidden;
                                    MonHocWD.btnXacNhanDoiTen.Visibility = Visibility.Hidden;
                                    return;
                                }

                                CmdString = "insert into MonHoc (TenMon, MaTruong, ApDung) values (N'" + monhoc + "', 1, 1)";
                                cmd = new SqlCommand(CmdString, con);
                                cmd.ExecuteNonQuery();
                                MessageBoxSuccessful messageBoxSuccessful = new MessageBoxSuccessful();
                                messageBoxSuccessful.ShowDialog();
                                con.Close();
                                MonHocWD.txtTenMH.Text = "";
                                MonHocWD.txtTenMH.IsEnabled = false;
                                MonHocWD.btnThemMonHoc.Visibility = Visibility.Visible;
                                MonHocWD.btnXacNhan.Visibility = Visibility.Hidden;
                                MonHocWD.btnXacNhanDoiTen.Visibility = Visibility.Hidden;
                                DataGridVisibility = false;
                                ProgressBarVisibility = true;
                                await LoadThongTinMonHoc();
                                DataGridVisibility = true;
                                ProgressBarVisibility = false;
                            }
                            catch (Exception)
                            {
                                MessageBoxFail messageBoxFail = new MessageBoxFail();
                                messageBoxFail.ShowDialog();
                            }
                        }
                    }
                }
            });
            SubjectSearchAll = new RelayCommand<TextBox>((parameter) => { return true; }, (parameter) =>
            {
                DanhSachMonHoc.Clear();
                IsLoadAll = true;
                using (SqlConnection con = new SqlConnection(ConnectionString.connectionString))
                {
                    try
                    {
                        try
                        {
                            con.Open();
                        }
                        catch (Exception)
                        {
                            MessageBoxFail messageBoxFail = new MessageBoxFail();
                            messageBoxFail.ShowDialog();
                            return;
                        }
                        string CmdString = "select * from MonHoc where ApDung = 1";
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
                        MessageBoxFail messageBoxFail = new MessageBoxFail();
                        messageBoxFail.ShowDialog();
                    }
                }

            });
        }
        public async Task LoadThongTinMonHoc()
        {
            DanhSachMonHoc = new ObservableCollection<Model.MonHoc>();

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

                    string CmdString = "select * from MonHoc where ApDung = 1";
                    SqlCommand cmd = new SqlCommand(CmdString, con);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        StudentManagement.Model.MonHoc monhoc = new StudentManagement.Model.MonHoc();
                        monhoc.MaMon = reader.GetInt32(0);
                        monhoc.TenMon = reader.GetString(1);
                        DanhSachMonHoc.Add(monhoc);
                    }

                    con.Close();
                }
                catch (Exception )
                {
                    MessageBoxFail messageBoxFail = new MessageBoxFail();
                    messageBoxFail.ShowDialog();
                }
            }
        }

    }
}
