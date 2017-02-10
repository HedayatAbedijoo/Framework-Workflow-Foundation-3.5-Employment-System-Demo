<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="RejectApplicant.aspx.cs" Inherits="AspHostApplication.RejectApplicant" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 400px">
        <tr>
            <td colspan="2" style="font-size: x-large">
                <span style="color: #0066CC">
                <i><b>RejectApplicant</b></i></span>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="font-size: x-large">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <asp:CheckBox ID="CheckBox1" runat="server" Text=" " />
            </td>
            <td dir="ltr">
                Did you close the applicant?
            </td>
        </tr>
        <tr>
            <td>
                <asp:CheckBox ID="CheckBox2" runat="server" Text=" " />
            </td>
            <td>
                ?Did you email him to announce
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td>
                    <asp:Button ID="btnOk" runat="server" Text="اقدام" Width="69px" OnClick="btnOk_Click" />
                </td>
        </tr>
    </table>
</asp:Content>
