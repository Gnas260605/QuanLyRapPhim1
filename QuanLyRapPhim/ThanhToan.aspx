<%@ Page Title="" Language="C#" MasterPageFile="~/LoaiPhim.Master" AutoEventWireup="true" CodeBehind="ThanhToan.aspx.cs" Inherits="QuanLyRapPhim.ThanhToan" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>XÁC NHẬN THANH TOÁN VÉ</h2>

    <div style="background-color: #f9f9f9; padding: 20px; border-radius: 6px; border: 1px solid #ddd; margin-bottom: 20px; line-height: 1.8; font-size: 15px;">
        <p><strong>Khách hàng:</strong> <asp:Label ID="lblKhachHang" runat="server"></asp:Label></p>
        <p><strong>Tên phim:</strong> <asp:Label ID="lblTenPhim" runat="server" Font-Bold="True"></asp:Label></p>
        <p><strong>Suất chiếu:</strong> <asp:Label ID="lblSuatChieu" runat="server"></asp:Label></p>
        <p><strong>Phòng chiếu:</strong> <asp:Label ID="lblPhongChieu" runat="server"></asp:Label></p>
        <p><strong>Ghế chọn:</strong> <asp:Label ID="lblGheChon" runat="server" ForeColor="Blue" Font-Bold="True"></asp:Label></p>
        <p style="font-size: 18px; border-top: 1px solid #ddd; padding-top: 10px; margin-top: 15px;">
            <strong>Tổng số tiền:</strong> 
            <span style="color: red; font-weight: bold;"><asp:Label ID="lblTongTien" runat="server"></asp:Label> VND</span>
        </p>
    </div>

    <asp:Label ID="lblThongBao" runat="server" ForeColor="Red" Font-Italic="True" style="display: block; margin-bottom: 15px;"></asp:Label>
    <asp:Label ID="lblThanhCong" runat="server" ForeColor="Green" Font-Bold="True" style="display: block; margin-bottom: 15px;"></asp:Label>

    <asp:Button ID="btnXacNhan" runat="server" Text="Xác nhận thanh toán" OnClick="btnXacNhan_Click" CssClass="btn" />
    <asp:Button ID="btnHuy" runat="server" Text="Hủy bỏ" OnClick="btnHuy_Click" CssClass="btn" style="background-color: #6c757d; margin-left: 10px;" />
</asp:Content>
