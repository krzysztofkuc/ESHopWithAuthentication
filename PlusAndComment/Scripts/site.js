
//Hide dialog basket if user click somewhere on the container
$(document).mouseup(function (e) {
    var myDialog = $("#basket");
    var container = $(".ui-dialog");

    if (myDialog.dialog("isOpen") === true) {
        if (!container.is(e.target) // if the target of the click isn't the container...
            && container.has(e.target).length === 0) // ... nor a descendant of the container
        {
            myDialog.dialog("close");
        }
    }
});

//Initialization
$(function () {
    $("#datepicker").datepicker();

    $("#basket").dialog({
        autoOpen: false,
        modal: false,
        dialogClass: "no-close",
        draggable: false,
        closeOnEscape: true,
        resizable: false,
        clickOutside: true,
        clickOutsideTrigger: "#trolleyItemsCountId",
        show: { effect: "blind", duration: 200 },
        hide: { effect: "blind", duration: 200 },
        position: {
            my: 'left bottom',
            at: 'left bottom',
            of: $('#trolleyItemsCountId'),
            open: function (event, ui) {
                $('.ui-widget-overlay').bind('click', function (event, ui) {
                    $('#basket').dialog('close');
                });
            }
        }
    });

    $('.ui-widget-overla').bind('click', function () {
        $("#basket").dialog('close');
        $('.ui-widget-overla').unbind();
    })

    $("#basket").mouseleave(function () {
        $("#basket").dialog("close");
    });
});

function UpdateTrolleyItemsCount(count) {
    $("#trolleyItemsCount").attr("data-count", count);
};

$('#example-one').hierarchySelect({
    width: 'auto'
});

//shopping cart

// Document.ready -> link up remove event handler
$(".RemoveLink").click(function () {
    // Get the id from the link
    var recordToDelete = $(this).attr("data-id");
    if (recordToDelete !== '') {
        // Perform the ajax post
        $.post("/ShoppingCart/RemoveFromCart", { "id": recordToDelete },
            function (data) {
                // Successful requests get here
                // Update the page elements
                if (data.ItemCount === 0) {
                    $('#row-' + data.DeleteId).fadeOut('slow');
                } else {
                    $('#item-count-' + data.DeleteId).text(data.ItemCount);
                }
                $('#cart-total').text(data.CartTotal);
                $('#update-message').text(data.Message);
                $('#cart-status').text('Cart (' + data.CartCount + ')');

                $('#trolleyItemsCount').attr("data-count", data.CartCount);
            });
    }
});

function animateAddToCartButton(count) {
    $(this).removeAttr('disabled').removeClass('ui-state-disabled');
    newButton = $(this).clone(true);
    moveButtonToTrolley(newButton, count);
}
    

function moveButtonToTrolley(element, count) {

    var clickedButtonIndex = element[0].id.match(/\d+/g).map(Number)[0];

    var addToCartButton = document.getElementById("addToCartLink" + clickedButtonIndex);
    var trolleyJSobj = document.getElementById("trolleyItemsCountId");
    var trolleyJQobj = $("#trolleyItemsCountId");
    var buttonAbsolutePos = getOffset(addToCartButton);
    var trolleyAbsolutePos = getOffset(trolleyJSobj);
    element.css({ position: 'fixed' });
    element.css({ top: buttonAbsolutePos.top, left: buttonAbsolutePos.left });
    element.appendTo('#cellAddToCart' + clickedButtonIndex);
    element.removeAttr("id");
       
    element.css('z-index', 500);

    element.animate({ left: trolleyAbsolutePos.left + 'px', top: trolleyAbsolutePos.top + 'px' },
        {
            duration: 700,
            complete: function () {
                $(this).toggle(
                    {
                        duration: 200,
                        effect: "scale",
                        direction: "horizontal",
                        complete: function () {
                            trolleyJQobj.addClass("run-animation");
                            troolleyClone = trolleyJQobj.clone(true);
                            trolleyJQobj.before(troolleyClone);
                            $("#trolleyItemsCount").attr("data-count", count);


                            trolleyJQobj.remove();
                            $(this).remove();
                            this.remove();
                            element.remove();
                    }
                });
            }
        },
    );
}

//get absoluite position from relative
function getOffset(el) {

    var curleft = curtop = 0;

    if (el.offsetParent) {
        do {
            curleft += el.offsetLeft;
            curtop += el.offsetTop;
        } while (el = el.offsetParent);

        return {
            left: curleft,
            top: curtop
        };
    }
}

$(document).ready(function () {
    $(".imageThumb_upload").change(function (e) {
        var formData = new FormData();
        //var totalFiles = e.target.files.length;
        for (var i = 0; i < e.target.files.length; i++) {
            var file = e.target.files[i];
            formData.append("imageUploadForm", file);
        }
        $.ajax({
            type: "POST",
            url: "Upload",
            data: formData,
            dataType: 'json',
            contentType: false,
            //enctype: 'multipart/form-data',
            processData: false,
            beforeSend: function (request, xhr) {
                $('#progressBar').text('');
                $('#progressBar').css('width', '0%');
            },
            success: function (post) {
                var photoNumber = GetNumberOfCurrentPhoto(e.target.id);

                $("#Path_" + photoNumber).val(post.PathRelative);
                $("#" + e.target.id + "_Preview").attr("src", post.PathRelative);

            },
            xhr: function () {
                //Get XmlHttpRequest object
                var xhr = $.ajaxSettings.xhr();
                //Set onprogress event handler
                xhr.upload.onprogress = function (data) {
                    var perc = Math.round((data.loaded / data.total) * 100);
                    $('#progressBar').text(perc + '%');
                    $('#progressBar').css('width', perc + '%');
                };
                return xhr;
            },
            error: function (error) {
                alert("errror");
            }
        });
    });

    //Initialize searchable dropdown with hierarchy
    $(document).ready(function () {
        $('#example-one').hierarchySelect({
            width: 200
        });
    });

    $('#addCategoryDropDown').click(function (e) {

        var value = $('ul#addCategoryDropDown').find('li.active').data('value');
        $("#parentId").val(value);
    });

    $('#addProductCategroyDropDown').click(function (e) {

        var value = $('ul#addProductCategroyDropDown').find('li.active').data('value');

        $("#catIdAddProduct").val(value);
    });

    $('#trolleyItemsCountId').click(function (event ) {
        
        $.ajax({
            type: 'GET',
            url: '/ShoppingCart/BasketThumb',
            dataType: 'html',
            beforeSend: function () { $("loaderBasketPreview").show(); },
            error: function (e) {
                $("loaderBasketPreview").hide();
            },
            success: function (partialViewData) {
                $("loaderBasketPreview").hide();

                $("#basket").html(partialViewData);
                $(".ui-dialog-titlebar").hide();     

                $("#basket").dialog("option", "position", {
                    my: "right top",
                    at: "right top+60",
                    of: $('#trolleyItemsCount') // this refers to the cliked element
                }).dialog("open");

            }
        });

        event.preventDefault();
        event.stopPropagation();
    });

});

    function GetNumberOfCurrentPhoto(id) {

        var lastslashindex = id.lastIndexOf('_');
        var result = id.substring(lastslashindex + 1);

        return result;
    }

const getCircularReplacer = () => {
  const seen = new WeakSet;
  return (key, value) => {
    if (typeof value === "object" && value !== null) {
      if (seen.has(value)) {
        return;
      }
      seen.add(value);
    }
    return value;
  };
};