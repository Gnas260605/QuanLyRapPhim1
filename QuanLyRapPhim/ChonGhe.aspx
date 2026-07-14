<%@ Page Title="" Language="C#" MasterPageFile="~/LoaiPhim.Master" AutoEventWireup="true" CodeBehind="ChonGhe.aspx.cs" Inherits="QuanLyRapPhim.ChonGhe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>CHỌN GHẾ NGỒI</h2>
    
    <div style="margin-bottom: 20px; background-color: #333; color: white; text-align: center; padding: 10px; font-weight: bold; border-radius: 4px;">
        MÀN HÌNH CHIẾU
    </div>

    <!-- Sơ đồ ghế -->
    <div class="seat-container">
        <asp:Repeater ID="rptGhe" runat="server" OnItemCommand="rptGhe_ItemCommand" OnItemDataBound="rptGhe_ItemDataBound">
            <ItemTemplate>
                <asp:Button ID="btnGhe" runat="server" 
                    Text='<%# Eval("Hang").ToString() + Eval("So").ToString() %>' 
                    CommandName="ChonGhe" 
                    CommandArgument='<%# Eval("MaGhe") %>'
                    CssClass='<%# GetSeatClass(Eval("LoaiGhe").ToString(), Eval("MaGhe").ToString()) %>' />
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <!-- Chú thích loại ghế -->
    <div style="display: flex; justify-content: center; gap: 20px; margin-top: 20px; font-size: 14px;">
        <div><span style="display: inline-block; width: 20px; height: 15px; background-color: #28a745; vertical-align: middle; border-radius: 2px;"></span> Ghế Thường</div>
        <div><span style="display: inline-block; width: 20px; height: 15px; background-color: #ffc107; vertical-align: middle; border-radius: 2px;"></span> Ghế VIP</div>
        <div><span style="display: inline-block; width: 20px; height: 15px; background-color: #dc3545; vertical-align: middle; border-radius: 2px;"></span> Ghế đã đặt</div>
        <div><span style="display: inline-block; width: 20px; height: 15px; background-color: #17a2b8; vertical-align: middle; border-radius: 2px;"></span> Ghế đang chọn</div>
    </div>

    <hr style="margin-top: 30px;" />

    <!-- Thông tin đặt vé tạm thời -->
    <div style="font-size: 16px; margin-top: 20px;">
        <p><strong>Ghế đã chọn:</strong> <asp:Label ID="lblGheDaChon" runat="server" Text="Chưa chọn ghế" ForeColor="Blue" Font-Bold="True"></asp:Label></p>
        <p><strong>Tạm tính:</strong> <span style="color: red; font-weight: bold; font-size: 18px;"><asp:Label ID="lblTongTien" runat="server" Text="0"></asp:Label> VND</span></p>
        
        <asp:Button ID="btnTiepTuc" runat="server" Text="Tiếp tục thanh toán" OnClick="btnTiepTuc_Click" CssClass="btn" style="margin-top: 10px;" />
        <br />
        <asp:Label ID="lblThongBao" runat="server" ForeColor="Red" Font-Italic="True"></asp:Label>
    </div>
</asp:Content>
