
$(function () {
    $('.datepicker').datetimepicker();
});

$(function () {
    $("#datepicker").datepicker();
});

function UpdateTrolleyItemsCount(count) {
    $("#trolleyItemsCount").attr("data-count", count);
};

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
       
        element.css('z-index', 30000000000000000000);

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
        //el = el.getBoundingClientRect();

        //return {
        //    left: el.left + window.scrollX,
        //    top: el.top + window.scrollY
        //};

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
                //var type = post.Type;
                //var gifUrl = post.postUrl;
                //var imgUrl = post.ImageUrl;
                //var referenceUrl = post.ReferenceUrl;

                var photoNumber = GetNumberOfCurrentPhoto(e.target.id);

                $("#Path_" + photoNumber).val(post.PathRelative);


                $("#" + e.target.id + "_Preview").attr("src", post.PathRelative);

                //$("#referenceUrl").val(referenceUrl);
                //$("#AddNewPostContent").show();
                //$('#addPostButton').attr("disabled", false);
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


        //Initialize searchable dropdown with hierarchy
        $(document).ready(function () {

            $('#your-expertise-hierarchy').hierarchySelect({

                hierarchy: true,

                search: true,

                width: 250

            });

        });
    });


    $('#trolleyItemsCountId').hover(function () {

        //do something

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
                $('#basket').html(partialViewData);
            }
        });
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

