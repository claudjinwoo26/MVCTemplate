$(document).ready(function () {
    loadDataTableCategory();
});


function loadDataTableCategory() {
    dataTable = $('#categoryTable').DataTable({ // ensure naming consistency
        "ajax": { url: '/Admin/Category/GetAllCategory' },
        "columns": [
            { data: 'nameCategory', "autowidth": true },
            { data: 'codeCategory', "autowidth": true },
            {
                data: 'idCategory',
                "render": function (data, type, full, meta) {
                    return `<div class="w-75 btn-group" role="group">
                                    <button type="button" data-id="${data}" data-name="${full.nameCategory}" data-code="${full.codeCategory}" class="btn-shadow btn btn-info" data-bs-toggle="modal" data-bs-target="#updateModal"> <i class="lnr-pencil"></i> Edit</button>
                                    <a onClick="Delete('/Admin/Category/Delete/${data}')" class="btn-shadow btn btn-danger mx-3"> <i class="lnr-trash"></i> Delete</a>
                                </div>`;
                },
                width: "25%", className: "text-center", orderable: false
            }
        ]
    });
}

// Call loadDataTable() to initialize your DataTable when the page loads

$('#updateModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var idCategory = button.data('id'); // must be same name with data-
    var nameCategory = button.data('name');
    var codeCategory = button.data('code');
    var modal = $(this);
    modal.find('.modal-body #idCategory').val(idCategory);
    modal.find('.modal-body #nameCategory').val(nameCategory);
    modal.find('.modal-body #codeCategory').val(codeCategory);
});

