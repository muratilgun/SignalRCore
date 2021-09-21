
$(() => {

    var connection = new signalR.HubConnectionBuilder().withUrl("/signalrServer").build();
    connection.start();
    connection.on("LoadProduct", function () {
        LoadProdData();
    })
    LoadProdData();
    function LoadProdData() {
        var tr = '';
        $.ajax({
            url: '/Product/GetProducts',
            method: 'GET',
            success: (result) => {
                $.each(result, (k, v) => {
                    tr += `<tr>
                        <td>${v.ProdName}</td>
                        <td>${v.Category}</td>
                        <td>${v.UnitPrice}</td>
                        <td>${v.StockQty}</td>
                        <td>
                            <a href='../Products/Edit?id=${v.ProdId}'>Edit</a>
                            <a href='../Products/Edit?id=${v.ProdId}'>Details</a>
                            <a href='../Products/Edit?id=${v.ProdId}'>Delete</a>
                        </td>
                    </tr>`

                })
                $("#tableBody").html(tr);

            },
            error: (error) => {
                console.log(error)
            }
        });
    }

})