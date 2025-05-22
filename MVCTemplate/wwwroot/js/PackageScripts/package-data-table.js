$(document).ready(function () {
    loadDataTable();
});


function loadDataTable() {
    dataTable = $('#packageTable').DataTable({ // ensure naming consistency
        "ajax": { url: '/Admin/Package/GetAllPackages' },
        "columns": [
            { data: 'name', "autowidth": true },
            { data: 'description', "autowidth": true },
            { data: 'priority', "autowidth": true },
            {
                data: 'id',
                "render": function (data, type, full, meta) {
                    return `<div class="w-75 btn-group" role="group">
                                    <button type="button" data-id="${data}" data-name="${full.name}" data-description="${full.description}" data-priority="${full.priority}" class="btn-shadow btn btn-info" data-bs-toggle="modal" data-bs-target="#updateModal"> <i class="lnr-pencil"></i> Edit</button>
                                    <a onClick="Delete('/Admin/Package/Delete/${data}')" class="btn-shadow btn btn-danger mx-3"> <i class="lnr-trash"></i> Delete</a>
                                </div>`;
                },
                width: "25%", className: "text-center", orderable: false
            }
        ]
    });
}

$('#updateModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var id = button.data('id'); // must be same name with data-
    var name = button.data('name');
    var description = button.data('description');
    var priority = button.data('priority');
    var modal = $(this);
    modal.find('.modal-body #id').val(id);
    modal.find('.modal-body #name').val(name);
    modal.find('.modal-body #description').val(description);
    modal.find('.modal-body #priority').val(priority);
});