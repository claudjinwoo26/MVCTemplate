$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#productTable').DataTable({
        "ajax": { url: '/Admin/Product/GetAllProducts' },
        "columns": [
            { data: 'name', "autowidth": true },
            { data: 'description', "autowidth": true },
            { data: 'quantity', "autowidth": true },
            {
                data: 'id',
                "render": function (data, type, full, meta) {
                    return `<div class="w-75 btn-group" role="group">
                                    <button type="button" data-id="${data}" data-name="${full.name}" class="btn-shadow btn btn-info" data-bs-toggle="modal" data-bs-target="#updateModal"> <i class="lnr-pencil"></i> Edit</button>
                                    <a onClick="Delete('/GeneralSetup/country/delete/${data}')" class="btn-shadow btn btn-danger mx-3"> <i class="lnr-trash"></i> Delete</a>
                                </div>`;

                },
                width: "25%", className: "text-center", orderable: false
            }
        ]
    });
}
