<%@ Page Title="" Language="C#" MasterPageFile="~/LoaiPhim.Master" AutoEventWireup="true" CodeBehind="Phim.aspx.cs" Inherits="QuanLyRapPhim.Phim" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>DANH SÁCH PHIM</h2>

    <%-- Ô tìm kiếm phim theo tên --%>
    <div style="display: flex; gap: 10px; max-width: 560px; margin-bottom: 18px;">
        <asp:TextBox ID="txtTimKiem" runat="server" CssClass="form-control" placeholder="Tìm phim theo tên..."></asp:TextBox>
        <asp:Button ID="btnTimKiem" runat="server" Text="Tìm" CssClass="btn" OnClick="btnTimKiem_Click" />
        <asp:Button ID="btnXoaTim" runat="server" Text="Xóa lọc" CssClass="btn"
            style="background: #4a5568; box-shadow: none;" OnClick="btnXoaTim_Click"
            CausesValidation="false" Visible="false" />
    </div>

    <asp:Label ID="lblKetQua" runat="server" Visible="false"
        style="display: block; margin-bottom: 14px; color: #a0aec0;"></asp:Label>

    <asp:DataList ID="DataList1" runat="server" RepeatColumns="3" Width="100%">
        <ItemTemplate>
            <div class="movie-item">
                <asp:Image ID="Image1" runat="server" ImageUrl='<%# "~/img/" + Eval("HinhAnh") %>' />
                <div class="movie-title">
                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("TenPhim") %>'></asp:Label>
                </div>
                <div class="movie-genre">
                    Thể loại: <asp:Label ID="Label2" runat="server" Text='<%# Eval("TheLoai") %>'></asp:Label>
                </div>
                <div style="font-size: 12px; margin-bottom: 10px;">
                    Thời lượng: <asp:Label ID="Label3" runat="server" Text='<%# Eval("ThoiLuong") %>'></asp:Label> phút
                </div>
                <asp:Button ID="Button1" runat="server" Text="Xem chi tiết" 
                    PostBackUrl='<%# "ChiTietPhim.aspx?MaPhim=" + Eval("MaPhim") %>' 
                    CssClass="btn" />
            </div>
        </ItemTemplate>
    </asp:DataList>

    <asp:Label ID="lblKhongCoPhim" runat="server" Visible="false" Font-Italic="True"
        Text="Không tìm thấy phim phù hợp." style="display: block; margin-top: 20px; color: #e2e8f0;"></asp:Label>
</asp:Content>
