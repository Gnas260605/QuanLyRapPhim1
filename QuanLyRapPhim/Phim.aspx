<%@ Page Title="" Language="C#" MasterPageFile="~/LoaiPhim.Master" AutoEventWireup="true" CodeBehind="Phim.aspx.cs" Inherits="QuanLyRapPhim.Phim" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>DANH SÁCH PHIM</h2>
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
</asp:Content>
