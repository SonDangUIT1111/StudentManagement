﻿namespace StudentManagement.Model
{
    public class HeThongDiem
    { 
        private int _maDiem;
        public int MaDiem { get { return _maDiem; } set { _maDiem = value; } }
        private int _hocKy;
        public int HocKy { get { return _hocKy; } set { _hocKy = value; } }
        private int _maLop;
        public int MaLop { get { return _maLop; } set { _maLop = value; } }
        
        private int _maMon;
        public int MaMon { get { return _maMon; } set { _maMon = value; } }
        
        private int _maHocSinh;
        public int MaHocSinh { get { return _maHocSinh; } set { _maHocSinh = value; } }
        private decimal _diem15Phut;
        public decimal Diem15Phut { get { return _diem15Phut; } set { _diem15Phut = value; } }
        private decimal _diem1Tiet;
        public decimal Diem1Tiet { get { return _diem1Tiet; } set { _diem1Tiet = value; } }
        private decimal _diemTB;
        public decimal DiemTB { get { return _diemTB; } set { _diemTB = value; } }
        private bool _xepLoai;
        public bool XepLoai { get { return _xepLoai; } set { _xepLoai = value; } }
        private bool _trangThai;
        public bool TrangThai { get { return _trangThai; } set { _trangThai = value; } }

    }
}
