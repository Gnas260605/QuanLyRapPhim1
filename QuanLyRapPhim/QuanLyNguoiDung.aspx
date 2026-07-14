<%@ Page Title="Quản Lý Tài Khoản" Language="C#" MasterPageFile="~/LoaiPhim.Master" AutoEventWireup="true" CodeBehind="QuanLyNguoiDung.aspx.cs" Inherits="QuanLyRapPhim.QuanLyNguoiDung" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>QUẢN LÝ TÀI KHOẢN</h2>

    <div class="ql-form">
        <h3 style="margin:0 0 18px;color:#ff7b00;font-size:17px">
            <asp:Label ID="lblFormTitle" runat="server" Text="Thêm tài khoản mới"></asp:Label>
        </h3>

        <asp:HiddenField ID="hdnMaNguoiDung" runat="server" />

        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label>Họ tên *</label>
                    <asp:TextBox ID="txtHoTen" runat="server" CssClass="form-control" placeholder="Nhập họ và tên..."></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvHoTen" runat="server" ControlToValidate="txtHoTen"
                        ErrorMessage="Họ tên không được trống!" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label>Email *</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="email@example.com" TextMode="Email"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                        ErrorMessage="Email không được trống!" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
            </div>
        </div>

        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label>Số điện thoại</label>
                    <asp:TextBox ID="txtSoDienThoai" runat="server" CssClass="form-control" placeholder="0901234567"></asp:TextBox>
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label>Mật khẩu *</label>
                    <asp:TextBox ID="txtMatKhau" runat="server" CssClass="form-control" TextMode="Password" placeholder="Nhập mật khẩu..."></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvMatKhau" runat="server" ControlToValidate="txtMatKhau"
                        ErrorMessage="Mật khẩu không được trống!" ForeColor="Red" Display="Dynamic"
                        Enabled="false"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label>Vai trò *</label>
                    <asp:DropDownList ID="ddlVaiTro" runat="server" CssClass="form-control">
                        <asp:ListItem Value="Staff">Staff - Nhân viên</asp:ListItem>
                        <asp:ListItem Value="Admin">Admin - Quản trị viên</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
        </div>

        <div style="margin-top:16px">
            <asp:Button ID="btnSave" runat="server" Text="Lưu tài khoản" CssClass="btn" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Hủy" CssClass="btn"
                style="background:#4a5568;margin-left:10px;box-shadow:none"
                OnClick="btnCancel_Click" CausesValidation="false" />
        </div>
    </div>


    <asp:Label ID="lblMsg" runat="server" CssClass="status-msg"></asp:Label>

    <div class="grid-container">
        <asp:GridView ID="gvNguoiDung" runat="server" AutoGenerateColumns="False" Width="100%"
            GridLines="None" CellPadding="10" ForeColor="#e2e8f0"
            OnRowCommand="gvNguoiDung_RowCommand" style="border-collapse:collapse">
            <HeaderStyle BackColor="#12121c" ForeColor="#a0aec0" Font-Bold="True" Height="44px" />
            <RowStyle BackColor="#1e1e2d" Height="40px" />
            <AlternatingRowStyle BackColor="#171725" />
            <Columns>
                <asp:BoundField DataField="MaNguoiDung" HeaderText="Mã" ItemStyle-Width="50px" />
                <asp:BoundField DataField="HoTen" HeaderText="Họ Tên" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                <asp:BoundField DataField="SoDienThoai" HeaderText="SĐT" ItemStyle-Width="110px" />
                <asp:TemplateField HeaderText="Vai Trò" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <span class='role-badge <%# Eval("VaiTro").ToString() == "Admin" ? "role-admin" : "role-staff" %>'>
                            <%# Eval("VaiTro") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Thao tác" ItemStyle-Width="130px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnEdit" runat="server" Text="Sửa" CommandName="EditND"
                            CommandArgument='<%# Eval("MaNguoiDung") %>' CssClass="action-link action-edit" CausesValidation="false"></asp:LinkButton>
                        <asp:LinkButton ID="btnDelete" runat="server" Text="Xóa" CommandName="DeleteND"
                            CommandArgument='<%# Eval("MaNguoiDung") %>' CssClass="action-link action-delete"
                            OnClientClick="return confirm('Xóa tài khoản này?');" CausesValidation="false"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
