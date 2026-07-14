<%@ Page Title="Quản Lý Lịch Chiếu" Language="C#" MasterPageFile="~/LoaiPhim.Master" AutoEventWireup="true" CodeBehind="QuanLyLichChieu.aspx.cs" Inherits="QuanLyRapPhim.QuanLyLichChieu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>QUẢN LÝ LỊCH CHIẾU</h2>

    <div class="ql-form">
        <h3 style="margin:0 0 18px;color:#ff7b00;font-size:17px">
            <asp:Label ID="lblFormTitle" runat="server" Text="Thêm lịch chiếu mới"></asp:Label>
        </h3>

        <asp:HiddenField ID="hdnMaLichChieu" runat="server" />

        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label>Phim *</label>
                    <asp:DropDownList ID="ddlPhim" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label>Phòng chiếu *</label>
                    <asp:DropDownList ID="ddlPhong" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
            </div>
        </div>

        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label>Ngày chiếu * (yyyy-MM-dd)</label>
                    <asp:TextBox ID="txtNgayChieu" runat="server" CssClass="form-control" placeholder="2025-12-25"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvNgay" runat="server" ControlToValidate="txtNgayChieu"
                        ErrorMessage="Ngày chiếu không được trống!" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label>Giờ bắt đầu * (HH:mm)</label>
                    <asp:TextBox ID="txtGioBatDau" runat="server" CssClass="form-control" placeholder="19:30"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvGio" runat="server" ControlToValidate="txtGioBatDau"
                        ErrorMessage="Giờ không được trống!" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label>Giá vé cơ bản (VNĐ) *</label>
                    <asp:TextBox ID="txtGiaVe" runat="server" CssClass="form-control" placeholder="80000"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvGia" runat="server" ControlToValidate="txtGiaVe"
                        ErrorMessage="Giá vé không được trống!" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="cvGia" runat="server" ControlToValidate="txtGiaVe"
                        Operator="DataTypeCheck" Type="Integer" ErrorMessage="Giá phải là số!" ForeColor="Red" Display="Dynamic"></asp:CompareValidator>
                </div>
            </div>
        </div>

        <div style="margin-top:16px">
            <asp:Button ID="btnSave" runat="server" Text="Lưu lịch chiếu" CssClass="btn" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Hủy" CssClass="btn"
                style="background:#4a5568;margin-left:10px;box-shadow:none"
                OnClick="btnCancel_Click" CausesValidation="false" />
        </div>
    </div>

    <asp:Label ID="lblMsg" runat="server" CssClass="status-msg"></asp:Label>

    <div class="grid-container">
        <asp:GridView ID="gvLichChieu" runat="server" AutoGenerateColumns="False" Width="100%"
            GridLines="None" CellPadding="10" ForeColor="#e2e8f0"
            OnRowCommand="gvLichChieu_RowCommand" style="border-collapse:collapse">
            <HeaderStyle BackColor="#12121c" ForeColor="#a0aec0" Font-Bold="True" Height="44px" />
            <RowStyle BackColor="#1e1e2d" Height="40px" />
            <AlternatingRowStyle BackColor="#171725" />
            <Columns>
                <asp:BoundField DataField="MaLichChieu" HeaderText="Mã LC" ItemStyle-Width="60px" />
                <asp:BoundField DataField="TenPhim" HeaderText="Tên Phim" />
                <asp:BoundField DataField="TenPhong" HeaderText="Phòng" ItemStyle-Width="110px" />
                <asp:TemplateField HeaderText="Ngày Chiếu" ItemStyle-Width="110px">
                    <ItemTemplate><%# string.Format("{0:dd/MM/yyyy}", Eval("NgayChieu")) %></ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="GioBatDau" HeaderText="Giờ" ItemStyle-Width="70px" />
                <asp:TemplateField HeaderText="Giá Vé" ItemStyle-Width="110px">
                    <ItemTemplate><span style="color:#ff7b00"><%# string.Format("{0:N0}", Eval("GiaVeCoBan")) %>đ</span></ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="SoVeDat" HeaderText="Đã đặt" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center" />
                <asp:TemplateField HeaderText="Thao tác" ItemStyle-Width="130px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnEdit" runat="server" Text="Sửa" CommandName="EditLC"
                            CommandArgument='<%# Eval("MaLichChieu") %>' CssClass="action-link action-edit" CausesValidation="false"></asp:LinkButton>
                        <asp:LinkButton ID="btnDelete" runat="server" Text="Xóa" CommandName="DeleteLC"
                            CommandArgument='<%# Eval("MaLichChieu") %>' CssClass="action-link action-delete"
                            OnClientClick="return confirm('Xóa lịch chiếu này?');" CausesValidation="false"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
