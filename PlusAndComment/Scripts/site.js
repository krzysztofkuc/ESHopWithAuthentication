function showleaveComment(id, isAuth) {
    if (!isAuth) {
        alert("Nie jesteś zalogowany")
        return false;
    }

    var obj = $("#leaveCommentPartial_" + id);

    var display = obj.css("display");
    if (display != "none")
    {
        obj.attr("style", "display:none");
    }
    else
    {
        obj.attr("style", "display:block");
    }

    return false;
}

function showleaveMainComment(id, isAuth) {
    if (!isAuth) {
        alert("Nie jesteś zalogowany")
        return false;
    }

    var obj = $("#leaveMainCommentPartial_" + id);

    var display = obj.css("display");
    if (display != "none") {
        obj.attr("style", "display:none");
    }
    else {
        obj.attr("style", "display:block");
    }

    return false;
}

function UpdateTrolleyItemsCount(count) {
    $("#trolleyItemsCount").attr("data-count", count);
};

//shopping cart

    // Document.ready -> link up remove event handler
    $(".RemoveLink").click(function () {
        // Get the id from the link
        var recordToDelete = $(this).attr("data-id");
        if (recordToDelete != '') {
            // Perform the ajax post
            $.post("/ShoppingCart/RemoveFromCart", { "id": recordToDelete },
                function (data) {
                    // Successful requests get here
                    // Update the page elements
                    if (data.ItemCount == 0) {
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
        //$(this).preventDefault();

        newButton = $(this).clone(true);
        moveButtonToTrolley(newButton, count);
    }
    

    function moveButtonToTrolley(element, count) {

        var addToCartButton = document.getElementById("addToCartLink1");
        var trolleyJSobj = document.getElementById("trolleyItemsCountId");
        var trolleyJQobj = $("#trolleyItemsCountId");
        var buttonAbsolutePos = getOffset(addToCartButton);
        var trolleyAbsolutePos = getOffset(trolleyJSobj);

        element.css({ position: 'absolute' });
        element.attr("z-index", "100000");
        element.removeAttr("id");
        element.css({ top: buttonAbsolutePos.top, left: buttonAbsolutePos.left });
        element.appendTo('#cellAddToCart1');

        element.animate({ left: trolleyAbsolutePos.left + 'px', top: trolleyAbsolutePos.top + 30 + 'px' },
            {
                duration: 500,
                complete: function () {
                    $(this).children(":first").toggle(
                        {
                            duration: 500,
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
        el = el.getBoundingClientRect();

        return {
            left: el.left + window.scrollX,
            top: el.top + window.scrollY
        }
    }

