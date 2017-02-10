<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FlowPanel.ascx.cs" Inherits="AspHostApplication.FlowPanel" %>
<asp:DropDownList ID="DropDownList1" runat="server">
</asp:DropDownList>
<br />
<asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="اقدام" />
<br />
<br />
<br />
<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Cartable.aspx">برو به کارتابل</asp:HyperLink>