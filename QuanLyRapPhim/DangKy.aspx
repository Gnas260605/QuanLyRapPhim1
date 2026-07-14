<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DangKy.aspx.cs" Inherits="QuanLyRapPhim.DangKy" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Đăng ký tài khoản</title>
    <link href="style.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="register-box">
            <div class="register-title">ĐĂNG KÝ TÀI KHOẢN</div>
            
            <div class="form-group">
                <label>Họ và tên *</label>
                <asp:TextBox ID="txtHoTen" runat="server" CssClass="form-control" placeholder="Nhập họ tên..."></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvHoTen" runat="server" ControlToValidate="txtHoTen" ErrorMessage="Họ tên không được để trống!" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <div class="form-group">
                <label>Email *</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Nhập email..."></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email không được để trống!" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ErrorMessage="Định dạng email sai!" ForeColor="Red" Display="Dynamic"></asp:RegularExpressionValidator>
            </div>

            <div class="form-group">
                <label>Số điện thoại *</label>
                <asp:TextBox ID="txtSoDienThoai" runat="server" CssClass="form-control" placeholder="Nhập số điện thoại..."></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvSoDienThoai" runat="server" ControlToValidate="txtSoDienThoai" ErrorMessage="SĐT không được để trống!" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
            
            <div class="form-group">
                <label>Mật khẩu *</label>
                <asp:TextBox ID="txtMatKhau" runat="server" CssClass="form-control" TextMode="Password" placeholder="Nhập mật khẩu..."></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvMatKhau" runat="server" ControlToValidate="txtMatKhau" ErrorMessage="Mật khẩu không được để trống!" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <div class="form-group">
                <label>Nhập lại mật khẩu *</label>
                <asp:TextBox ID="txtXacNhanMatKhau" runat="server" CssClass="form-control" TextMode="Password" placeholder="Nhập lại mật khẩu..."></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvXacNhanMatKhau" runat="server" ControlToValidate="txtXacNhanMatKhau" ErrorMessage="Vui lòng nhập lại mật khẩu!" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:CompareValidator ID="cvConfirm" runat="server" ControlToValidate="txtXacNhanMatKhau" ControlToCompare="txtMatKhau" ErrorMessage="Mật khẩu không khớp!" ForeColor="Red" Display="Dynamic"></asp:CompareValidator>
            </div>

            <asp:Label ID="lblThongBao" runat="server" ForeColor="Red" Font-Italic="True" style="display: block; margin-bottom: 10px;"></asp:Label>
            <asp:Label ID="lblThanhCong" runat="server" ForeColor="Green" Font-Bold="True" style="display: block; margin-bottom: 10px;"></asp:Label>

            <asp:Button ID="btnDangKy" runat="server" Text="Đăng ký" OnClick="btnDangKy_Click" CssClass="btn" Width="100%" />
            
            <div style="margin-top: 15px; text-align: center; font-size: 13px;">
                Đã có tài khoản? <a href="DangNhap.aspx">Đăng nhập ngay</a> <br />
                <a href="Phim.aspx" style="display: inline-block; margin-top: 10px;">Quay lại trang chủ</a>
            </div>
        </div>
    </form>
</body>
</html>
