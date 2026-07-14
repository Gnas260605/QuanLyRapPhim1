<%@ Page Title="" Language="C#" MasterPageFile="~/LoaiPhim.Master" AutoEventWireup="true" CodeBehind="AdminPhim.aspx.cs" Inherits="QuanLyRapPhim.AdminPhim" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .admin-form {
            background-color: #1e1e2d;
            padding: 25px;
            border-radius: 12px;
            border: 1px solid rgba(255,255,255,0.05);
            margin-bottom: 30px;
        }
        .form-row {
            display: flex;
            gap: 20px;
            margin-bottom: 15px;
        }
        .form-col {
            flex: 1;
        }
        .grid-container {
            background-color: #1e1e2d;
            padding: 20px;
            border-radius: 12px;
            border: 1px solid rgba(255,255,255,0.05);
            overflow-x: auto;
        }
        .action-link {
            font-weight: 600;
            text-decoration: none;
            margin-right: 15px;
            transition: color 0.3s;
        }
        .action-edit {
            color: #4fd1c5;
        }
        .action-edit:hover {
            color: #319795;
            text-decoration: underline;
        }
        .action-delete {
            color: #e53e3e;
        }
        .action-delete:hover {
            color: #c53030;
            text-decoration: underline;
        }
        .status-msg {
            display: block;
            margin-bottom: 15px;
            font-weight: 500;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>QUẢN LÝ DANH SÁCH PHIM (ADMIN)</h2>

    <div class="admin-form">
        <h3 style="margin-top: 0; margin-bottom: 20px; color: #ff7b00; font-size: 18px;">
            <asp:Label ID="lblFormTitle" runat="server" Text="Thêm phim mới"></asp:Label>
        </h3>
        
        <asp:HiddenField ID="hdnMaPhim" runat="server" />

        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label>Tên phim *</label>
                    <asp:TextBox ID="txtTenPhim" runat="server" CssClass="form-control" placeholder="Nhập tên phim..."></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvTenPhim" runat="server" ControlToValidate="txtTenPhim" ErrorMessage="Tên phim không được trống!" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label>Thể loại *</label>
                    <asp:TextBox ID="txtTheLoai" runat="server" CssClass="form-control" placeholder="Hành động, Viễn tưởng..."></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvTheLoai" runat="server" ControlToValidate="txtTheLoai" ErrorMessage="Thể loại không được trống!" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
            </div>
        </div>

        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label>Thời lượng (Phút) *</label>
                    <asp:TextBox ID="txtThoiLuong" runat="server" CssClass="form-control" placeholder="Nhập số phút..."></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvThoiLuong" runat="server" ControlToValidate="txtThoiLuong" ErrorMessage="Thời lượng không được trống!" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="cvThoiLuong" runat="server" ControlToValidate="txtThoiLuong" Operator="DataTypeCheck" Type="Integer" ErrorMessage="Thời lượng phải là số nguyên!" ForeColor="Red" Display="Dynamic"></asp:CompareValidator>
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label>Đạo diễn</label>
                    <asp:TextBox ID="txtDaoDien" runat="server" CssClass="form-control" placeholder="Nhập tên đạo diễn..."></asp:TextBox>
                </div>
            </div>
        </div>

        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label>Ngày khởi chiếu *</label>
                    <asp:TextBox ID="txtNgayKhoiChieu" runat="server" CssClass="form-control" placeholder="yyyy-MM-dd"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvNgay" runat="server" ControlToValidate="txtNgayKhoiChieu" ErrorMessage="Ngày khởi chiếu không được trống!" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label>Tên file hình ảnh *</label>
                    <asp:TextBox ID="txtHinhAnh" runat="server" CssClass="form-control" placeholder="Ví dụ: endgame.jpg"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvHinh" runat="server" ControlToValidate="txtHinhAnh" ErrorMessage="Tên ảnh không được trống!" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
            </div>
        </div>

        <div class="form-group">
            <label>Mô tả phim</label>
            <asp:TextBox ID="txtMoTa" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" placeholder="Nhập mô tả phim..."></asp:TextBox>
        </div>

        <div style="margin-top: 20px;">
            <asp:Button ID="btnSave" runat="server" Text="Lưu Phim" CssClass="btn" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Hủy" CssClass="btn" style="background: #4a5568; margin-left: 10px; box-shadow: none;" OnClick="btnCancel_Click" CausesValidation="false" />
        </div>
    </div>

    <asp:Label ID="lblMsg" runat="server" CssClass="status-msg" ForeColor="Green"></asp:Label>

    <div class="grid-container">
        <asp:GridView ID="gvPhim" runat="server" AutoGenerateColumns="False" Width="100%" 
            GridLines="None" CellPadding="10" ForeColor="#e2e8f0"
            OnRowCommand="gvPhim_RowCommand" style="border-collapse: collapse;">
            <HeaderStyle BackColor="#12121c" ForeColor="#a0aec0" Font-Bold="True" Height="45px" />
            <RowStyle BackColor="#1e1e2d" Height="40px" />
            <AlternatingRowStyle BackColor="#171725" />
            <Columns>
                <asp:BoundField DataField="MaPhim" HeaderText="Mã Phim" HeaderStyle-HorizontalAlign="Left" ItemStyle-Width="80px" />
                <asp:BoundField DataField="TenPhim" HeaderText="Tên Phim" HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="TheLoai" HeaderText="Thể Loại" HeaderStyle-HorizontalAlign="Left" ItemStyle-Width="120px" />
                <asp:BoundField DataField="ThoiLuong" HeaderText="Thời Lượng" HeaderStyle-HorizontalAlign="Left" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="DaoDien" HeaderText="Đạo Diễn" HeaderStyle-HorizontalAlign="Left" />
                <asp:TemplateField HeaderText="Ngày Chiếu" HeaderStyle-HorizontalAlign="Left" ItemStyle-Width="120px">
                    <ItemTemplate>
                        <%# Eval("NgayKhoiChieu", "{0:dd/MM/yyyy}") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="HinhAnh" HeaderText="Hình Ảnh" HeaderStyle-HorizontalAlign="Left" />
                <asp:TemplateField HeaderText="Hành Động" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnEdit" runat="server" Text="Sửa" CommandName="EditMovie" CommandArgument='<%# Eval("MaPhim") %>' CssClass="action-link action-edit" CausesValidation="false"></asp:LinkButton>
                        <asp:LinkButton ID="btnDelete" runat="server" Text="Xóa" CommandName="DeleteMovie" CommandArgument='<%# Eval("MaPhim") %>' CssClass="action-link action-delete" OnClientClick="return confirm('Bạn có chắc chắn muốn xóa phim này không?');" CausesValidation="false"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
