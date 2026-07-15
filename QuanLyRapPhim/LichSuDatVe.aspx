<%@ Page Title="" Language="C#" MasterPageFile="~/LoaiPhim.Master" AutoEventWireup="true" CodeBehind="LichSuDatVe.aspx.cs" Inherits="QuanLyRapPhim.LichSuDatVe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>LỊCH SỬ ĐẶT VÉ</h2>

    <asp:Label ID="lblChuaDangNhap" runat="server" Text="Vui lòng đăng nhập để xem lịch sử đặt vé!" ForeColor="Red" Font-Italic="True" Visible="False"></asp:Label>

    <asp:Label ID="lblMsg" runat="server" Font-Bold="True" style="display: block; margin-bottom: 12px;"></asp:Label>

    <asp:GridView ID="gvLichSu" runat="server" AutoGenerateColumns="False" Width="100%" CellPadding="10" ForeColor="#e2e8f0" GridLines="None" OnRowCommand="gvLichSu_RowCommand" style="border-collapse: collapse; margin-top: 15px;">
        <HeaderStyle BackColor="#12121c" ForeColor="#a0aec0" Font-Bold="True" Height="45px" />
        <RowStyle BackColor="#1e1e2d" Height="40px" />
        <AlternatingRowStyle BackColor="#171725" />
        <Columns>
            <asp:BoundField DataField="MaDatVe" HeaderText="Mã Vé" />
            <asp:BoundField DataField="TenPhim" HeaderText="Tên Phim" />
            <asp:TemplateField HeaderText="Suất Chiếu">
                <ItemTemplate>
                    <%# Eval("NgayChieu", "{0:dd/MM/yyyy}") %> - <%# Eval("GioBatDau") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="TenPhong" HeaderText="Phòng" />
            <asp:TemplateField HeaderText="Ghế">
                <ItemTemplate>
                    <span style="color: #4fd1c5;"><%# Eval("DanhSachGhe") %></span>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="NgayDat" HeaderText="Ngày Mua" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
            <asp:TemplateField HeaderText="Tổng Tiền">
                <ItemTemplate>
                    <strong style="color: #ff7b00;"><%# Eval("TongTien", "{0:0,0}") %> VND</strong>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="TrangThai" HeaderText="Trạng Thái" />
            <asp:TemplateField HeaderText="">
                <ItemTemplate>
                    <asp:LinkButton ID="btnHuyVe" runat="server" Text="Hủy vé"
                        CommandName="CancelVe" CommandArgument='<%# Eval("MaDatVe") %>'
                        Visible='<%# Convert.ToInt32(Eval("CoTheHuy")) == 1 %>'
                        OnClientClick="return confirm('Bạn chắc chắn muốn hủy vé này? Thao tác không thể hoàn tác.');"
                        style="color: #e53e3e; font-weight: 600; text-decoration: none;"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <asp:Label ID="lblThongBaoTrong" runat="server" Text="Bạn chưa mua vé nào!" Font-Italic="True" style="display: block; margin-top: 20px;" Visible="False"></asp:Label>
</asp:Content>
