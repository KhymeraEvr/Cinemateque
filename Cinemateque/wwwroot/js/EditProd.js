function getUserOrders() {
    var user = document.getElementById("UserNameEd").value
    $.ajax({
        type: "GET",
        url: "Edit?name=" + user
    });
}

var currentPage = 1;

function loadNextPage() {
    currentPage = currentPage + 1;
    $.ajax({
        type: "GET",
        url: "Prods/" + currentPage,
    });
}


function addDiscount() {
    var prod = document.getElementById("prodName").value;
    var discount = document.getElementById("discount").value;
    var dat = JSON.stringify({
        "ProdName": prod,
        "Discount": discount
    });

    $.ajax({
        type: 'POST',
        url: "AddDiscount",
        data: dat,
        contentType: "application/json",
        success: function (data) {
            alert("Successfuly aded discount")
        }

    });
}

function editOrder(data) {
    var order = $(data).data('assigned-id')
    var status = data.value;
    var dat = JSON.stringify({
        OrderId: order,
        newStatus: status
    });
    $.ajax({
        type: 'POST',
        url: "EditOrder",
        data: dat,
        contentType: "application/json",
        success: function (data) {
            alert("Successfuly updated order")
        }

    });
}