﻿using StudentManagement.Views.HocSinh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace StudentManagement.ViewModel.HocSinh
{
    internal class TrangChuViewModel:BaseViewModel
    {
        //declare variable

        private int _idHocSinh;
        public int IdHocSinh { get { return _idHocSinh; } set { _idHocSinh = value; } }

        //declare Pages
        public LopHoc LopHocPage { get; set; }
        public ThongTinHocSinh ThongTinHocSinhPage { get; set; }
        public ThongTinTruong ThongTinTruongPage { get; set; }
        public DiemSo XemDiemPage { get; set; }


        //declare ICommand
        public ICommand SwitchThongTinHocSinh { get; set; }
        public ICommand SwitchLopHoc { get; set; }
        public ICommand SwitchThongTinTruong { get; set; }
        public ICommand SwitchXemDiem { get; set; }

        public TrangChuViewModel()
        {
            LopHocPage = new LopHoc();
            ThongTinHocSinhPage = new ThongTinHocSinh();
            ThongTinTruongPage = new ThongTinTruong();
            XemDiemPage = new DiemSo();

            //define ICommand
            SwitchThongTinHocSinh = new RelayCommand<Frame>((parameter) => { return true; }, (parameter) =>
            {
                parameter.Content = ThongTinHocSinhPage;
            });
            SwitchLopHoc = new RelayCommand<Frame>((parameter) => { return true; }, (parameter) =>
            {
                parameter.Content = LopHocPage;
            });
            SwitchThongTinTruong = new RelayCommand<Frame>((parameter) => { return true; }, (parameter) =>
            {
                parameter.Content = ThongTinTruongPage;
            });
            SwitchXemDiem = new RelayCommand<Frame>((parameter) => { return true; }, (parameter) =>
            {
                parameter.Content = XemDiemPage;
            });
        }
    }
}
