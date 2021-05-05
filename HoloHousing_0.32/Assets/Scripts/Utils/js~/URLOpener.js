mergeInto(LibraryManager.library, {
    OpenURL: function (message) { 
        message = UTF8ToString(message);  
        var gameCanvas = document.getElementById("UT_CANVAS");  

        var scheme = "twitter://post?message=" + message;
        var pc_site = "https://twitter.com/intent/tweet?text=" + message;

        var userAgent = navigator.userAgent.toLowerCase();
        if (gameCanvas != null)  {
            
            if(userAgent.indexOf("android") !== -1 
                || userAgent.indexOf("iphone") !== -1
                || userAgent.indexOf("ipad") !== -1)
            {
                if(window.open(scheme, "_blank"))
                {}
                else
                {
                    window.location.href = scheme;
                    setTimeout(function() 
                    {
                        window.location.href = pc_site;
                    }, 500);
                }
            }
            else
            {
                if(window.open(pc_site, "_blank","width=400,height=300"))
                {}
                else
                {
                    window.location.href = pc_site;
                }
            }

            
            
        } else {
            console.error("UT_CANVAS not found, was it renamed?");
        }
    },
    GetQuery: function () { 
        var url = new URL(window.location.href);
        var params = url.searchParams;
        var s = "";
        if(params.has("room"))
        {
            s = params.get("room");
        }
        var size = lengthBytesUTF8(s)+1;
        var ptr = _malloc(size);
        stringToUTF8(s,ptr,size);
        return ptr;
    }

});