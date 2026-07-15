<%@ Page Title="Đổi mật khẩu" Language="C#" MasterPageFile="~/LoaiPhim.Master" AutoEventWireup="true" CodeBehind="DoiMatKhau.aspx.cs" Inherits="QuanLyRapPhim.DoiMatKhau" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>ĐỔI MẬT KHẨU</h2>

    <div class="ql-form" style="max-width: 520px;">
        <asp:Label ID="lblMsg" runat="server" CssClass="status-msg"></asp:Label>

        <div class="form-group">
            <label>Mật khẩu hiện tại *</label>
            <asp:TextBox ID="txtMatKhauCu" runat="server" CssClass="form-control" TextMode="Password" placeholder="Nhập mật khẩu hiện tại..."></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvCu" runat="server" ControlToValidate="txtMatKhauCu" ValidationGroup="DMK"
                ErrorMessage="Vui lòng nhập mật khẩu hiện tại!" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
        </div>

        <div class="form-group">
            <label>Mật khẩu mới *</label>
            <asp:TextBox ID="txtMatKhauMoi" runat="server" CssClass="form-control" TextMode="Password" placeholder="Ít nhất 6 ký tự..."></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvMoi" runat="server" ControlToValidate="txtMatKhauMoi" ValidationGroup="DMK"
                ErrorMessage="Vui lòng nhập mật khẩu mới!" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="revMoi" runat="server" ControlToValidate="txtMatKhauMoi" ValidationGroup="DMK"
                ValidationExpression=".{6,}" ErrorMessage="Mật khẩu mới phải từ 6 ký tự trở lên!" ForeColor="Red" Display="Dynamic"></asp:RegularExpressionValidator>
        </div>

        <div class="form-group">
            <label>Xác nhận mật khẩu mới *</label>
            <asp:TextBox ID="txtXacNhan" runat="server" CssClass="form-control" TextMode="Password" placeholder="Nhập lại mật khẩu mới..."></asp:TextBox>
            <asp:CompareValidator ID="cvXacNhan" runat="server" ControlToValidate="txtXacNhan" ControlToCompare="txtMatKhauMoi" ValidationGroup="DMK"
                ErrorMessage="Xác nhận mật khẩu không khớp!" ForeColor="Red" Display="Dynamic"></asp:CompareValidator>
        </div>

        <div style="margin-top: 16px;">
            <asp:Button ID="btnDoi" runat="server" Text="Đổi mật khẩu" CssClass="btn" OnClick="btnDoi_Click" ValidationGroup="DMK" />
        </div>
    </div>
</asp:Content>
