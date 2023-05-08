﻿using StudentManagement.Views.GiamHieu;
using System.Windows.Controls;
using System.Windows.Input;

namespace StudentManagement.ViewModel.GiamHieu
{
    public class TrangChuViewModel : BaseViewModel
    {
        //declare variable

        private int _idGiamHieu;
        public int IdGiamHieu { get { return _idGiamHieu; } set { _idGiamHieu = value; } }

        //declare Pages
        public BaoCaoMonHoc BaoCaoPage { get; set; }
        public LopHoc LopHocPage { get; set; }
        public ThayDoiQuyDinh ThayDoiQuyDinhPage { get; set; }
        public ThongTinGiaoVien ThongTinGiaoVienPage { get; set; }
        public ThongTinHocSinh ThongTinHocSinhPage { get; set; }
        public ThongTinTruong ThongTinTruongPage { get; set; }
        public QuanLiDiemSo QuanLiDiemSoPage { get; set; }


        //declare ICommand
        public ICommand SwitchThongTinHocSinh { get; set; }
        public ICommand SwitchThongTinGiaoVien { get; set; }
        public ICommand SwitchLopHoc { get; set; }
        public ICommand SwitchThongTinTruong { get; set; }
        public ICommand SwitchBaoCao { get; set; }
        public ICommand SwitchThayDoiQuyDinh { get; set; }
        public ICommand SwitchQuanLyBangDiem { get; set; }


        public TrangChuViewModel()
        {
            BaoCaoPage = new BaoCaoMonHoc();
            LopHocPage = new LopHoc();
            ThayDoiQuyDinhPage = new ThayDoiQuyDinh();
            ThongTinGiaoVienPage = new ThongTinGiaoVien();
            ThongTinHocSinhPage = new ThongTinHocSinh();
            ThongTinTruongPage = new ThongTinTruong();
            QuanLiDiemSoPage = new QuanLiDiemSo();

            //define ICommand
            SwitchThongTinHocSinh = new RelayCommand<Frame>((parameter) => { return true; }, (parameter) =>
            {
                parameter.Content = ThongTinHocSinhPage;
            });
            SwitchThongTinGiaoVien = new RelayCommand<Frame>((parameter) => { return true; }, (parameter) =>
            {
                parameter.Content = ThongTinGiaoVienPage;
            });
            SwitchLopHoc = new RelayCommand<Frame>((parameter) => { return true; }, (parameter) =>
            {
                parameter.Content = LopHocPage;
            });
            SwitchThongTinTruong = new RelayCommand<Frame>((parameter) => { return true; }, (parameter) =>
            {
                parameter.Content = ThongTinTruongPage;
            });
            SwitchBaoCao = new RelayCommand<Frame>((parameter) => { return true; }, (parameter) =>
            {
                parameter.Content = BaoCaoPage;
            });
            SwitchThayDoiQuyDinh = new RelayCommand<Frame>((parameter) => { return true; }, (parameter) =>
            {
                parameter.Content = ThayDoiQuyDinhPage;
            });
            SwitchQuanLyBangDiem = new RelayCommand<Frame>((parameter) => { return true; }, (parameter) =>
            {
                parameter.Content = QuanLiDiemSoPage;
            });

        }
    }
}
