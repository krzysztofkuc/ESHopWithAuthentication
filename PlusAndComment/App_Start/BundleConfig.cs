﻿using System.Web;
using System.Web.Optimization;

namespace PlusAndComment
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/angularjs").Include(
            //"~/Scripts/angular.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                        "~/Scripts/site.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.validate.unobtrusive.js",
                        "~/Scripts/jquery.unobtrusive*"));

            //Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/moment.js",
                        "~/Scripts/moment-with-locales.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/bootstrap.min.js",
                        "~/Scripts/bootstrap-datetimepicker.min.js",
                        "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/buttons.css",
                      "~/Content/Site.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/animate.css"
                      ));
        }
    }
}
