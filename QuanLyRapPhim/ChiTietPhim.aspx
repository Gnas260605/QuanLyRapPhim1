<%@ Page Title="" Language="C#" MasterPageFile="~/LoaiPhim.Master" AutoEventWireup="true" CodeBehind="ChiTietPhim.aspx.cs" Inherits="QuanLyRapPhim.ChiTietPhim" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>CHI TIẾT PHIM</h2>
    
    <div style="display: flex; margin-bottom: 30px;">
        <div style="margin-right: 20px;">
            <asp:Image ID="imgPoster" runat="server" Width="200px" Height="300px" style="object-fit: cover; border: 1px solid #ccc; border-radius: 4px;" />
        </div>
        <div>
            <h3 style="margin-top: 0;"><asp:Label ID="lblTenPhim" runat="server" Font-Bold="True" Font-Size="20px"></asp:Label></h3>
            <p><strong>Thể loại:</strong> <asp:Label ID="lblTheLoai" runat="server"></asp:Label></p>
            <p><strong>Thời lượng:</strong> <asp:Label ID="lblThoiLuong" runat="server"></asp:Label> phút</p>
            <p><strong>Đạo diễn:</strong> <asp:Label ID="lblDaoDien" runat="server"></asp:Label></p>
            <p><strong>Khởi chiếu:</strong> <asp:Label ID="lblNgayKhoiChieu" runat="server"></asp:Label></p>
            <p><strong>Mô tả:</strong> <asp:Label ID="lblMoTa" runat="server"></asp:Label></p>
        </div>
    </div>

    <hr />

    <h3>DANH SÁCH SUẤT CHIẾU</h3>
    <asp:DataList ID="dlSuatChieu" runat="server" Width="100%">
        <ItemTemplate>
            <div style="border: 1px solid rgba(255, 255, 255, 0.05); padding: 20px; margin-bottom: 15px; border-radius: 12px; display: flex; justify-content: space-between; align-items: center; background-color: #1e1e2d; box-shadow: 0 4px 12px rgba(0,0,0,0.15);">
                <div>
                    <strong style="color: #a0aec0;">Phòng chiếu:</strong> <asp:Label ID="lblPhong" runat="server" Text='<%# Eval("TenPhong") %>'></asp:Label> <br />
                    <strong style="color: #a0aec0;">Ngày chiếu:</strong> <asp:Label ID="lblNgay" runat="server" Text='<%# Eval("NgayChieu", "{0:dd/MM/yyyy}") %>'></asp:Label> <br />
                    <strong style="color: #a0aec0;">Giờ chiếu:</strong> <asp:Label ID="lblGio" runat="server" Text='<%# Eval("GioBatDau") %>' ForeColor="#ff2e54" Font-Bold="true"></asp:Label>
                </div>
                <div>
                    <strong style="color: #ff7b00; font-size: 18px;"><asp:Label ID="lblGia" runat="server" Text='<%# Eval("GiaVeCoBan", "{0:0,0}") %>'></asp:Label> VND</strong>
                </div>
                <div>
                    <asp:Button ID="btnDatVe" runat="server" Text="Đặt vé" 
                        PostBackUrl='<%# "ChonGhe.aspx?MaLichChieu=" + Eval("MaLichChieu") %>' 
                        CssClass="btn" />
                </div>
            </div>
        </ItemTemplate>
    </asp:DataList>
    <asp:Label ID="lblThongBaoLC" runat="server" ForeColor="Red" Font-Italic="True" Text="Hiện tại phim chưa có suất chiếu nào!" Visible="False"></asp:Label>
</asp:Content>
