<%@ Page Title="Quản Lý Phòng Chiếu" Language="C#" MasterPageFile="~/LoaiPhim.Master" AutoEventWireup="true" CodeBehind="QuanLyPhongChieu.aspx.cs" Inherits="QuanLyRapPhim.QuanLyPhongChieu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>QUẢN LÝ PHÒNG CHIẾU &amp; GHẾ</h2>

    <asp:Label ID="lblMsg" runat="server" CssClass="status-msg"></asp:Label>

    <%-- ═══════════ PHẦN 1: THÊM / SỬA PHÒNG ═══════════ --%>
    <div class="ql-form">
        <h3 style="margin:0 0 18px;color:#ff7b00;font-size:17px">
            <asp:Label ID="lblFormTitle" runat="server" Text="Thêm phòng chiếu mới"></asp:Label>
        </h3>

        <asp:HiddenField ID="hdnMaPhong" runat="server" />

        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label>Tên phòng *</label>
                    <asp:TextBox ID="txtTenPhong" runat="server" CssClass="form-control" placeholder="Ví dụ: Phòng Chiếu 1"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvTenPhong" runat="server" ControlToValidate="txtTenPhong" ValidationGroup="Phong"
                        ErrorMessage="Tên phòng không được trống!" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label>Sức chứa (ghế) *</label>
                    <asp:TextBox ID="txtSucChua" runat="server" CssClass="form-control" placeholder="Ví dụ: 60"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSucChua" runat="server" ControlToValidate="txtSucChua" ValidationGroup="Phong"
                        ErrorMessage="Sức chứa không được trống!" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="cvSucChua" runat="server" ControlToValidate="txtSucChua" ValidationGroup="Phong"
                        Operator="DataTypeCheck" Type="Integer" ErrorMessage="Sức chứa phải là số!" ForeColor="Red" Display="Dynamic"></asp:CompareValidator>
                </div>
            </div>
        </div>

        <div style="margin-top:16px">
            <asp:Button ID="btnSavePhong" runat="server" Text="Lưu phòng" CssClass="btn" OnClick="btnSavePhong_Click" ValidationGroup="Phong" />
            <asp:Button ID="btnCancelPhong" runat="server" Text="Hủy" CssClass="btn"
                style="background:#4a5568;margin-left:10px;box-shadow:none"
                OnClick="btnCancelPhong_Click" CausesValidation="false" />
        </div>
    </div>

    <div class="grid-container">
        <asp:GridView ID="gvPhong" runat="server" AutoGenerateColumns="False" Width="100%"
            GridLines="None" CellPadding="10" ForeColor="#e2e8f0"
            OnRowCommand="gvPhong_RowCommand" style="border-collapse:collapse">
            <HeaderStyle BackColor="#12121c" ForeColor="#a0aec0" Font-Bold="True" Height="44px" />
            <RowStyle BackColor="#1e1e2d" Height="40px" />
            <AlternatingRowStyle BackColor="#171725" />
            <Columns>
                <asp:BoundField DataField="MaPhong" HeaderText="Mã Phòng" ItemStyle-Width="80px" />
                <asp:BoundField DataField="TenPhong" HeaderText="Tên Phòng" />
                <asp:BoundField DataField="SucChua" HeaderText="Sức Chứa" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="SoGheThucTe" HeaderText="Số Ghế Đã Tạo" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center" />
                <asp:TemplateField HeaderText="Thao tác" ItemStyle-Width="220px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnQuanLyGhe" runat="server" Text="Quản lý ghế" CommandName="ManageSeats"
                            CommandArgument='<%# Eval("MaPhong") %>' CssClass="action-link" style="color:#9b59b6" CausesValidation="false"></asp:LinkButton>
                        <asp:LinkButton ID="btnEdit" runat="server" Text="Sửa" CommandName="EditPhong"
                            CommandArgument='<%# Eval("MaPhong") %>' CssClass="action-link action-edit" CausesValidation="false"></asp:LinkButton>
                        <asp:LinkButton ID="btnDelete" runat="server" Text="Xóa" CommandName="DeletePhong"
                            CommandArgument='<%# Eval("MaPhong") %>' CssClass="action-link action-delete"
                            OnClientClick="return confirm('Xóa phòng này? Toàn bộ ghế của phòng cũng sẽ bị xóa.');" CausesValidation="false"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <%-- ═══════════ PHẦN 2: QUẢN LÝ GHẾ CỦA PHÒNG ĐANG CHỌN ═══════════ --%>
    <asp:Panel ID="pnlGhe" runat="server" Visible="false" style="margin-top:34px">
        <div class="ql-form">
            <h3 style="margin:0 0 6px;color:#4fd1c5;font-size:17px">
                <i class="fa fa-th"></i> Quản lý ghế —
                <asp:Label ID="lblTenPhongGhe" runat="server"></asp:Label>
            </h3>
            <p style="color:#a0aec0;font-size:13px;margin:0 0 18px">
                Công cụ sinh ghế tự động: mỗi hàng đặt tên A, B, C... và đánh số ghế từ 1. Ghế đã tồn tại sẽ được giữ nguyên.
            </p>

            <asp:HiddenField ID="hdnMaPhongGhe" runat="server" />

            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label>Số hàng ghế * (A, B, C...)</label>
                        <asp:TextBox ID="txtSoHang" runat="server" CssClass="form-control" placeholder="Ví dụ: 6"></asp:TextBox>
                        <asp:CompareValidator ID="cvSoHang" runat="server" ControlToValidate="txtSoHang" ValidationGroup="Ghe"
                            Operator="DataTypeCheck" Type="Integer" ErrorMessage="Số hàng phải là số!" ForeColor="Red" Display="Dynamic"></asp:CompareValidator>
                    </div>
                </div>
                <div class="form-col">
                    <div class="form-group">
                        <label>Số ghế mỗi hàng *</label>
                        <asp:TextBox ID="txtSoGheMoiHang" runat="server" CssClass="form-control" placeholder="Ví dụ: 10"></asp:TextBox>
                        <asp:CompareValidator ID="cvSoGhe" runat="server" ControlToValidate="txtSoGheMoiHang" ValidationGroup="Ghe"
                            Operator="DataTypeCheck" Type="Integer" ErrorMessage="Số ghế phải là số!" ForeColor="Red" Display="Dynamic"></asp:CompareValidator>
                    </div>
                </div>
                <div class="form-col">
                    <div class="form-group">
                        <label>Loại ghế *</label>
                        <asp:DropDownList ID="ddlLoaiGhe" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Thường" Value="Thường"></asp:ListItem>
                            <asp:ListItem Text="VIP" Value="VIP"></asp:ListItem>
                            <asp:ListItem Text="Đôi" Value="Đôi"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>

            <div style="margin-top:16px">
                <asp:Button ID="btnSinhGhe" runat="server" Text="Sinh ghế" CssClass="btn" OnClick="btnSinhGhe_Click" ValidationGroup="Ghe" />
                <asp:Button ID="btnXoaTatCaGhe" runat="server" Text="Xóa toàn bộ ghế" CssClass="btn"
                    style="background:#e53e3e;margin-left:10px;box-shadow:none"
                    OnClick="btnXoaTatCaGhe_Click" CausesValidation="false"
                    OnClientClick="return confirm('Xóa TOÀN BỘ ghế của phòng này? (ghế đã có vé đặt sẽ không xóa được)');" />
                <asp:Button ID="btnDongGhe" runat="server" Text="Đóng" CssClass="btn"
                    style="background:#4a5568;margin-left:10px;box-shadow:none"
                    OnClick="btnDongGhe_Click" CausesValidation="false" />
            </div>
        </div>

        <div class="grid-container">
            <asp:GridView ID="gvGhe" runat="server" AutoGenerateColumns="False" Width="100%"
                GridLines="None" CellPadding="10" ForeColor="#e2e8f0"
                OnRowCommand="gvGhe_RowCommand" style="border-collapse:collapse">
                <HeaderStyle BackColor="#12121c" ForeColor="#a0aec0" Font-Bold="True" Height="44px" />
                <RowStyle BackColor="#1e1e2d" Height="38px" />
                <AlternatingRowStyle BackColor="#171725" />
                <Columns>
                    <asp:BoundField DataField="MaGhe" HeaderText="Mã Ghế" ItemStyle-Width="80px" />
                    <asp:TemplateField HeaderText="Vị trí" ItemStyle-Width="90px">
                        <ItemTemplate><%# Eval("Hang") %><%# Eval("So") %></ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Hang" HeaderText="Hàng" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="So" HeaderText="Số" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="LoaiGhe" HeaderText="Loại Ghế" ItemStyle-Width="120px" />
                    <asp:TemplateField HeaderText="Thao tác" ItemStyle-Width="90px" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDeleteGhe" runat="server" Text="Xóa" CommandName="DeleteGhe"
                                CommandArgument='<%# Eval("MaGhe") %>' CssClass="action-link action-delete"
                                OnClientClick="return confirm('Xóa ghế này?');" CausesValidation="false"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </asp:Panel>
</asp:Content>
