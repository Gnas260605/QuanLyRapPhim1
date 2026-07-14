<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/LoaiPhim.Master" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="QuanLyRapPhim.AdminDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>DASHBOARD QUẢN TRỊ</h2>

    <%-- ===== STAT CARDS ===== --%>
    <div class="dashboard-grid">
        <div class="stat-card revenue" id="divRevenueCard" runat="server">
            <div class="stat-content">
                <div class="stat-label">Tổng doanh thu</div>
                <div class="stat-value revenue"><asp:Label ID="lblDoanhThu" runat="server" Text="0"></asp:Label>đ</div>
                <div class="stat-sub">Từ vé đã thanh toán</div>
            </div>
            <span class="stat-icon" style="color: #ff7b00;"><i class="fa fa-money"></i></span>
        </div>
        <div class="stat-card tickets">
            <div class="stat-content">
                <div class="stat-label">Vé đã bán</div>
                <div class="stat-value tickets"><asp:Label ID="lblSoVe" runat="server" Text="0"></asp:Label></div>
                <div class="stat-sub">Tổng số ghế đã đặt</div>
            </div>
            <span class="stat-icon" style="color: #4fd1c5;"><i class="fa fa-ticket"></i></span>
        </div>
        <div class="stat-card movies">
            <div class="stat-content">
                <div class="stat-label">Số phim</div>
                <div class="stat-value movies"><asp:Label ID="lblSoPhim" runat="server" Text="0"></asp:Label></div>
                <div class="stat-sub">Phim trong hệ thống</div>
            </div>
            <span class="stat-icon" style="color: #a78bfa;"><i class="fa fa-film"></i></span>
        </div>
        <div class="stat-card users">
            <div class="stat-content">
                <div class="stat-label">Tài khoản</div>
                <div class="stat-value users"><asp:Label ID="lblSoTaiKhoan" runat="server" Text="0"></asp:Label></div>
                <div class="stat-sub">Admin + Staff</div>
            </div>
            <span class="stat-icon" style="color: #f6e05e;"><i class="fa fa-users"></i></span>
        </div>
    </div>


    <%-- ===== TOP PHIM + LỊCH CHIẾU HÔM NAY ===== --%>
    <div class="dash-row">
        <div class="dash-panel">
            <div class="panel-header">
                <span class="panel-icon" style="color: #d69e2e;"><i class="fa fa-trophy"></i></span>
                <span class="panel-title">Top Phim Đặt Nhiều Nhất</span>
            </div>
            <%-- Phiên bản Admin: Có doanh thu --%>
            <asp:Repeater ID="rptTopPhimAdmin" runat="server">
                <HeaderTemplate><table class="dash-table"><tr><th style="width:36px">#</th><th>Tên Phim</th><th style="width:70px;text-align:center">Vé</th><th style="width:120px;text-align:right">Doanh thu</th></tr></HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><span class="rank rank-n"><%# Container.ItemIndex + 1 %></span></td>
                        <td style="color:#fff;font-weight:500"><%# Eval("TenPhim") %></td>
                        <td style="text-align:center"><span class="badge badge-purple"><%# Eval("SoVe") %></span></td>
                        <td style="text-align:right;color:#ff7b00;font-weight:600"><%# string.Format("{0:N0}", Eval("DoanhThu")) %>đ</td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate></table></FooterTemplate>
            </asp:Repeater>
            <%-- Phiên bản Staff: Ẩn cột doanh thu --%>
            <asp:Repeater ID="rptTopPhimStaff" runat="server" Visible="false">
                <HeaderTemplate><table class="dash-table"><tr><th style="width:36px">#</th><th>Tên Phim</th><th style="width:70px;text-align:center">Vé đã đặt</th></tr></HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><span class="rank rank-n"><%# Container.ItemIndex + 1 %></span></td>
                        <td style="color:#fff;font-weight:500"><%# Eval("TenPhim") %></td>
                        <td style="text-align:center"><span class="badge badge-purple"><%# Eval("SoVe") %> vé</span></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate></table></FooterTemplate>
            </asp:Repeater>
            <asp:Panel ID="pnlTopPhimEmpty" runat="server" Visible="false" CssClass="empty-state">
                <div class="empty-icon"><i class="fa fa-film"></i></div><div>Chưa có dữ liệu đặt vé</div>
            </asp:Panel>
        </div>

        <div class="dash-panel">
            <div class="panel-header">
                <span class="panel-icon" style="color: #ff7b00;"><i class="fa fa-calendar"></i></span>
                <span class="panel-title">Lịch Chiếu Hôm Nay (<asp:Label ID="lblHomNay" runat="server"></asp:Label>)</span>
            </div>
            <asp:Repeater ID="rptLichChieu" runat="server">
                <HeaderTemplate><table class="dash-table"><tr><th>Phim</th><th style="width:70px">Giờ</th><th style="width:85px">Phòng</th><th style="width:65px;text-align:center">Đặt</th></tr></HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("TenPhim") %></td>
                        <td><span class="badge badge-orange"><%# Eval("GioBatDau") %></span></td>
                        <td style="color:#a78bfa"><%# Eval("TenPhong") %></td>
                        <td style="text-align:center"><%# Eval("SoVeDat") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate></table></FooterTemplate>
            </asp:Repeater>
            <asp:Panel ID="pnlLichEmpty" runat="server" Visible="false" CssClass="empty-state">
                <div class="empty-icon"><i class="fa fa-calendar"></i></div><div>Không có lịch chiếu hôm nay</div>
            </asp:Panel>
        </div>
    </div>


    <%-- ===== 10 ĐẶT VÉ GẦN NHẤT ===== --%>
    <div class="dash-row">
        <div class="dash-panel full-width">
            <div class="panel-header">
                <span class="panel-icon" style="color: #4fd1c5;"><i class="fa fa-list-alt"></i></span>
                <span class="panel-title">10 Đặt Vé Gần Nhất</span>
            </div>
            <%-- Phiên bản Admin: Có tổng tiền --%>
            <asp:Repeater ID="rptDatVeAdmin" runat="server">
                <HeaderTemplate>
                    <table class="dash-table">
                        <tr>
                            <th style="width:55px">Mã</th><th>Khách hàng</th><th>Phim</th>
                            <th style="width:100px">Ngày chiếu</th><th style="width:65px">Giờ</th>
                            <th style="width:55px;text-align:center">Ghế</th>
                            <th style="width:115px;text-align:right">Tổng tiền</th>
                            <th style="width:105px">Ngày đặt</th>
                            <th style="width:100px;text-align:center">Trạng thái</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td style="color:#718096">#<%# Eval("MaDatVe") %></td>
                        <td style="color:#fff;font-weight:500"><%# Eval("HoTen") %></td>
                        <td><%# Eval("TenPhim") %></td>
                        <td><%# string.Format("{0:dd/MM/yyyy}", Eval("NgayChieu")) %></td>
                        <td><%# Eval("GioBatDau") %></td>
                        <td style="text-align:center"><%# Eval("SoGhe") %></td>
                        <td style="text-align:right;color:#ff7b00;font-weight:600"><%# string.Format("{0:N0}", Eval("TongTien")) %>đ</td>
                        <td style="color:#718096;font-size:12px"><%# string.Format("{0:dd/MM HH:mm}", Eval("NgayDat")) %></td>
                        <td style="text-align:center"><span class="badge badge-success"><%# Eval("TrangThai") %></span></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate></table></FooterTemplate>
            </asp:Repeater>
            <%-- Phiên bản Staff: Ẩn cột tổng tiền --%>
            <asp:Repeater ID="rptDatVeStaff" runat="server" Visible="false">
                <HeaderTemplate>
                    <table class="dash-table">
                        <tr>
                            <th style="width:55px">Mã</th><th>Khách hàng</th><th>Phim</th>
                            <th style="width:100px">Ngày chiếu</th><th style="width:65px">Giờ</th>
                            <th style="width:55px;text-align:center">Số vé</th>
                            <th style="width:105px">Ngày đặt</th>
                            <th style="width:100px;text-align:center">Trạng thái</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td style="color:#718096">#<%# Eval("MaDatVe") %></td>
                        <td style="color:#fff;font-weight:500"><%# Eval("HoTen") %></td>
                        <td><%# Eval("TenPhim") %></td>
                        <td><%# string.Format("{0:dd/MM/yyyy}", Eval("NgayChieu")) %></td>
                        <td><%# Eval("GioBatDau") %></td>
                        <td style="text-align:center"><%# Eval("SoGhe") %></td>
                        <td style="color:#718096;font-size:12px"><%# string.Format("{0:dd/MM HH:mm}", Eval("NgayDat")) %></td>
                        <td style="text-align:center"><span class="badge badge-success"><%# Eval("TrangThai") %></span></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate></table></FooterTemplate>
            </asp:Repeater>
            <asp:Panel ID="pnlDatVeEmpty" runat="server" Visible="false" CssClass="empty-state">
                <div class="empty-icon"><i class="fa fa-file-text-o"></i></div><div>Chưa có giao dịch nào</div>
            </asp:Panel>
        </div>
    </div>

    <%-- ===== PHÒNG CHIẾU + TÀI KHOẢN ===== --%>
    <div class="dash-row">
        <div class="dash-panel" id="divPhongChieuPanel" runat="server">
            <div class="panel-header">
                <span class="panel-icon" style="color: #a78bfa;"><i class="fa fa-building-o"></i></span>
                <span class="panel-title">Danh Sách Phòng Chiếu</span>
            </div>
            <asp:Repeater ID="rptPhongChieu" runat="server">
                <HeaderTemplate><table class="dash-table"><tr><th>Tên phòng</th><th style="width:75px;text-align:center">Tổng ghế</th><th style="width:60px;text-align:center">VIP</th><th style="width:70px;text-align:center">Thường</th></tr></HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td style="color:#fff;font-weight:500"><%# Eval("TenPhong") %></td>
                        <td style="text-align:center"><span class="badge badge-teal"><%# Eval("TongGhe") %></span></td>
                        <td style="text-align:center;color:#d69e2e;font-weight:600"><%# Eval("GheVip") %></td>
                        <td style="text-align:center;color:#718096"><%# Eval("GheThuong") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate></table></FooterTemplate>
            </asp:Repeater>
        </div>

        <div class="dash-panel" id="divTaiKhoanPanel" runat="server">
            <div class="panel-header">
                <span class="panel-icon" style="color: #f6e05e;"><i class="fa fa-users"></i></span>
                <span class="panel-title">Tài Khoản Hệ Thống</span>
            </div>
            <asp:Repeater ID="rptTaiKhoan" runat="server">
                <HeaderTemplate><table class="dash-table"><tr><th>Họ tên</th><th>Email</th><th style="width:75px;text-align:center">Vai trò</th></tr></HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td style="color:#fff;font-weight:500"><%# Eval("HoTen") %></td>
                        <td style="color:#718096;font-size:12px"><%# Eval("Email") %></td>
                        <td style="text-align:center">
                            <span class='badge <%# Eval("VaiTro").ToString() == "Admin" ? "badge-orange" : "badge-purple" %>'><%# Eval("VaiTro") %></span>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate></table></FooterTemplate>
            </asp:Repeater>
        </div>
    </div>


    <%-- ===== QUICK LINKS ===== --%>
    <div class="dash-panel" style="margin-bottom:10px">
        <div class="panel-header">
            <span class="panel-icon" style="color: #68d391;"><i class="fa fa-bolt"></i></span>
            <span class="panel-title">Truy Cập Nhanh</span>
        </div>
        <div class="quick-links">
            <a href="AdminPhim.aspx" class="quick-link"><span class="quick-link-icon" style="color:#ff7b00"><i class="fa fa-film"></i></span> Quản lý Phim</a>
            <a href="QuanLyLichChieu.aspx" class="quick-link"><span class="quick-link-icon" style="color:#4fd1c5"><i class="fa fa-calendar"></i></span> Quản lý Lịch Chiếu</a>
            <a href="QuanLyNguoiDung.aspx" class="quick-link"><span class="quick-link-icon" style="color:#a78bfa"><i class="fa fa-user-circle"></i></span> Quản lý Tài Khoản</a>
        </div>
    </div>

</asp:Content>

