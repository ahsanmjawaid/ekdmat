﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="accomplished-requests.aspx.cs" Inherits="Khadmatcom.providers.accomplished_services" %>

<%@ Import Namespace="Khadmatcom" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="/Content/carousel-css/owl.theme.css" />
    <link rel="stylesheet" type="text/css" href="/Content/carousel-css/owl.carousel.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="server">
    <ul class="nav nav-tabs nav-arow myTab">
        <li class="main alif"><a href="<%= GetLocalizedUrl("") %>"><%= GetGlobalResourceObject("general.aspx","Home") %></a></li>
        <li class="sub active alif"><a href="javascript:{}">خدمات منفذة</a></li>
    </ul>
    <div id="chuu-owl" class="chuu owl-carousel owl-theme">
        <asp:ListView runat="server" ID="lvServiceRequest" SelectMethod="GetServiceRequests" ItemPlaceholderID="PlaceHolder1" GroupItemCount="3" ItemType="Khadmatcom.Data.Model.ServiceRequest">
            <GroupTemplate>
                <div class="item">
                    <div class="accordion clearfix" id="accordion4">
                        <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
                    </div>
                </div>
            </GroupTemplate>
            <ItemTemplate>
                <div class="panel">
                    <div class="accordion-heading">
                        <a class="clearfix accordion-toggle collapsed indicator" data-toggle="collapse" data-parent="#accordion4" href='#right<%# Item.Id %>' aria-expanded="false">

                            <div class="L-container">
                                <div class="L1">
                                    <span class="ni">رقم الطلب: <span class="red"><%# Item.Id %></span> </span>
                                    <span>الخدمة المطلوبة: <span class="blue"><%# GetServiceInfo(Item.ServiceId,LanguageId,"title") %></span> </span>
                                    <span>نوعها:<span class="blue"><%# GetServiceInfo(Item.ServiceId,LanguageId,"subcategory") %></span> </span>
                                </div>
                                <div class="clearfix"></div>
                                <div class="row L2">
                                    <div class="col-md-12 col-sm-12 col-xs-12 pull-right;">
                                        :تفاصيل الخدمة
                                    </div>
                                </div>
                                <div class="L3">
                                    <p>
                                        <%# Item.Details %>
                                    </p>
                                </div>
                            </div>

                        </a>

                    </div>
                    <div id="right<%# Item.Id %>" class="collapse" aria-expanded="false">
                        <div class="accordion-body clearfix" dir="rtl" style="direction: rtl;">
                            <div class="list-group L-container">
                                <asp:Repeater runat="server" ItemType="Khadmatcom.Data.Model.RequestsOptionsAnswer" DataSource='<%# Item.RequestsOptionsAnswers %>'>
                                    <ItemTemplate>
                                        <div class="col-md-6  col-sm-6 col-xs-12 pull-right">
                                            <div class="input-group">
                                                <label class="list-group-item-heading blue"><i class="fa fa-arrow-circle-o-left" aria-hidden="true"></i>&nbsp;<%# Item.RequestOption.Title %></label>
                                                &nbsp;<label><%# GetAnswer(Item.Value) %></label>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <div class="col-md-6  col-sm-6 col-xs-12 pull-right">
                                    <div class="input-group">
                                        <label class="list-group-item-heading blue"><i class="fa fa-arrow-circle-o-left blue" aria-hidden="true"></i>مدة التنفيذ</label>
                                        :
                                                       &nbsp; <span class=""><%# Item.TotalDuration %> يوم</span>
                                    </div>
                                </div>
                                <div class="col-md-6  col-sm-6 col-xs-12 pull-right">
                                    <div class="input-group">
                                        <label class="list-group-item-heading blue"><i class="fa fa-arrow-circle-o-left" aria-hidden="true"></i>طريقة الدفع</label>
                                        :
                                                       &nbsp; <span class=""><%# GetPaymentMethod(Item.PaymentMethod) %></span>
                                    </div>
                                </div>
                                <div class="col-md-6  col-sm-6 col-xs-12 pull-right">
                                    <div class="input-group">
                                        <label class="list-group-item-heading blue"><i class="fa fa-pencil-square-o" aria-hidden="true"></i>تعليق </label>
                                        :
                                                       &nbsp; <span class=""><%# Item.Notes %></span>
                                    </div>
                                </div>
                                <div class="col-md-6  col-sm-6 col-xs-12 pull-right">
                                    <label class="pull-right blue"><i class="fa fa-arrow-circle-o-left" aria-hidden="true"></i>المرفقات :</label>&nbsp;
                            <asp:ListView runat="server" DataSource="<%# Item.Attachments.Where(x=>x.IsOutput) %>" ItemType="Khadmatcom.Data.Model.Attachment">
                                <ItemTemplate>
                                    <a class="attach_url" target="_blank" href='<%# string.Format("/Attachments/{0}", Item.Path)%>'>المرفق <%# Container.DataItemIndex+1 %></a>
                                </ItemTemplate>
                                <ItemSeparatorTemplate>, </ItemSeparatorTemplate>
                            </asp:ListView>
                                </div>
                            </div>
                            <div class="clearfix"></div>
                            <div class="L-button" id="">
                                <button type="button" style="padding: 3px; opacity: 1; color: green;" class="btn btn-default disabled text-success ">سعر الخدمة:<%# Item.RequestProviders.First(x=>x.ProviderId==CurrentUser.Id).Price %>&nbsp;<span style="display: inline-block; float: left">ريال</span>&nbsp;  </button>
                                <a href="<%# GetLocalizedUrl(string.Format("providers/services-requests/{0}/request-details",Item.Id.EncodeNumber())) %>" class="editt hidden">Edit</a>
                            </div>

                        </div>
                    </div>
                </div>

            </ItemTemplate>
        </asp:ListView>



    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="js" runat="server">
    <script src="/Scripts/carousel-js/owl.carousel.js"></script>
    <script src="/Scripts/carousel-js/owl.carousel.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#chuu-owl").owlCarousel({

                navigation: true, // Show next and prev buttons
                slideSpeed: 300,
                paginationSpeed: 400,
                singleItem: true
            });



            $(".owl-prev").addClass("fa");
            $(".owl-prev").addClass("fa-chevron-left");
            $(".owl-prev").text("");

            $(".owl-next").addClass("fa");
            $(".owl-next").addClass("fa-chevron-right");
            $(".owl-next").text("");



        });
    </script>
</asp:Content>
