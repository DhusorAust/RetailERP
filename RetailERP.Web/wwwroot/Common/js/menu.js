
var MenuHelper = {
    InitMenu: function (menus) {
        MenuHelper.PopulateMenus(menus);
        $(".menu-nav a").each(function () {
            var curURL = (window.location.pathname + window.location.search);
            curURL = curURL.replace(/\/$/, "");
            curURL = decodeURIComponent(curURL);

            var href = $(this).attr('href');
            if (curURL === href) {
                $(this).closest('li').addClass('menu-item-open');
                $(this).closest('li').children("a").addClass('menu-link-active');
                $(this).closest('li').parents("li").addClass('menu-item-open');
                $(this).closest('li').parents("ul").css({ "display": "block" });
            }
        });

        $(".menu-nav a").each(function () {
            var curURL = (window.location.pathname + window.location.search);
            var href1 = $(this).attr('href1');
            var href2 = $(this).attr('href2');
            if ((curURL.indexOf(href1) >= 0) || (curURL.indexOf(href2) >= 0)) {
                $(this).closest('li').addClass('menu-item-open');
                $(this).closest('li').children("a").addClass('menu-link-active');
                $(this).closest('li').parents("li").find('.arrow').addClass('menu-item-open');
                $(this).closest('li').parents("li").addClass('menu-item-open');
                $(this).closest('li').parents("ul").css({ "display": "block" });
            }
        });
    },
    PopulateMenus: function (menus) {
        var menulink = ""
        menulink = ""
        for (var i = 0; i < menus.length; i++) {
            if (menus[i].ParentMenuID == null || menus[i].ParentMenuID == 0) {
                menulink += '<li class="menu-item menu-item-submenu" aria-haspopup="true" data-menu-toggle="hover" > '
                menulink += '<a href="javascript: ; " class="menu-link menu-toggle" > '
                menulink += '<span class="svg-icon menu-icon" > '
                menulink += '<i class="' + menus[i].MenuIcon + '" > </i>'
                menulink += '</span>'
                menulink += '<span class="menu-text" > ' + menus[i].MenuHead + ' </span>'
                menulink += '<i class="menu-arrow" > </i>'
                menulink += '</a>'
                menulink += MenuHelper.AddchiledMenu(menus[i], menus[i].MenuID, menus);
                menulink += '</li>';
            }
        }
        
        var finalMenulink = "<ul class='menu-nav'>"
        finalMenulink += '<li class="menu-section"><h2 class="menu-text" ><a style = "color:white !important; font-size: 25px; margin-bottom: 15px; margin-left: 0px;font-family: cursive;" href="/Security/Default"> Retail ERP</a> </h2><i class="menu-icon ki ki-bold-more-hor icon-md" > </i></li>'
        finalMenulink += menulink
        finalMenulink += "</ul>";

        $("#kt_aside_menu").append(finalMenulink);
    },
    AddchiledMenu: function (objMenuOrginal, menuId, objMenuList) {
        menulink = "<div class='menu-submenu' >"
        menulink += "<i class='menu-arrow' > </i>"
        menulink += "<ul class='menu-subnav' >"
        var added = false;
        for (var j = 0; j < objMenuList.length; j++) {
            if (objMenuList[j].ParentMenuID == menuId) {
                var hasChild = objMenuList.filter(function (ob) {
                    return (ob.ParentMenuID == objMenuList[j].MenuID);
                });
                if (hasChild.length > 0) {
                    menulink += '<li class="menu-item menu-item-submenu" aria-haspopup="true" data-menu-toggle="hover" > '
                    menulink += ' <a href="javascript:;" class="menu-link menu-toggle" > '
                    menulink += ' <i class="menu-bullet menu-bullet-line" >'
                    menulink += '<span></span>'
                    menulink += ' </i>'
                    menulink += ' <span class="menu-text" > ' + objMenuList[j].MenuHead + ' </span>'
                    menulink += ' <i class="menu-arrow" > </i>'
                    menulink += '</a>'
                    menulink += MenuHelper.AddchiledMenu(objMenuList[j], objMenuList[j].MenuID, objMenuList, "");
                    menulink += " </li>"
                }
                else {
                    menulink += '<li class="menu-item" aria-haspopup="true" > '
                    menulink += ' <a href="/' + objMenuList[j].Controller + '/' + objMenuList[j].ViewName + '?menuId=' + objMenuList[j].MenuID + '&pageName=' + objMenuList[j].PageName + '" class="menu-link" > '
                    menulink += ' <i class="fa fa-angle-right" >'
                    menulink += '<span></span>'
                    menulink += ' </i>'
                    menulink += ' <span class="menu-text" > ' + objMenuList[j].MenuHead + ' </span>'
                    menulink += '</a>'
                    menulink += MenuHelper.AddchiledMenu(objMenuList[j], objMenuList[j].MenuID, objMenuList);
                    menulink += " </li>"
                }
            }
        }
        menulink += ' </ul>'
        menulink += '</div>'
        menulink += " </li>";
        return menulink;
    }
}