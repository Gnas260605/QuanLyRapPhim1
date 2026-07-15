<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DangNhap.aspx.cs" Inherits="QuanLyRapPhim.DangNhap" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Đăng nhập hệ thống</title>
    <link href="style.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-box">
            <div class="login-title">ĐĂNG NHẬP</div>
            
            <div class="form-group">
                <label>Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Nhập email..."></asp:TextBox>
            </div>
            
            <div class="form-group">
                <label>Mật khẩu</label>
                <asp:TextBox ID="txtMatKhau" runat="server" CssClass="form-control" TextMode="Password" placeholder="Nhập mật khẩu..."></asp:TextBox>
            </div>

            <asp:Label ID="lblBaoLoi" runat="server" ForeColor="Red" Font-Italic="True" style="display: block; margin-bottom: 10px;"></asp:Label>

            <asp:Button ID="btnDangNhap" runat="server" Text="Đăng nhập" OnClick="btnDangNhap_Click" CssClass="btn" Width="100%" />
            
            <div style="margin-top: 15px; text-align: center; font-size: 13px;">
                <%-- Hệ thống không có đăng ký công khai, tài khoản do Admin cấp --%>
                Tài khoản do quản trị viên cấp. Liên hệ Admin nếu bạn chưa có tài khoản.<br />
                <a href="Phim.aspx" style="display: inline-block; margin-top: 10px;">Quay lại trang chủ</a>
            </div>
        </div>
    </form>
</body>
</html>
