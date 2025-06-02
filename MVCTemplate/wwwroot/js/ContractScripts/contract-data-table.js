
// Custom filter to handle min/max quantity filtering
$.fn.dataTable.ext.search.push(
    function (settings, data, dataIndex) {
        var min = parseInt($('#minQuantity').val(), 10);
        var max = parseInt($('#maxQuantity').val(), 10);
        var quantity = parseInt(data[2]) || 0;

        if ((isNaN(min) && isNaN(max)) ||
            (isNaN(min) && quantity <= max) ||
            (min <= quantity && isNaN(max)) ||
            (min <= quantity && quantity <= max)) {
            return true;
        }
        return false;
    }
);

$(document).ready(function () {
    loadDataTable();
});


// Step 2: Define the DataTable and rendering logic

function loadDataTable() {
    dataTable = $('#contractTable').DataTable({
        "ajax": { url: '/Admin/Contract/GetAllContracts' },
        "columns": [
            { data: 'name', "autowidth": true },
            { data: 'description', "autowidth": true },
            {
                data: 'validity',
                render: function (data) {
                    const date = new Date(data);
                    const options = { year: 'numeric', month: 'long', day: 'numeric' };
                    return date.toLocaleDateString('en-US', options); // e.g. "June 2, 2025"
                },
                "autowidth": true
            },
            { data: 'personId', "autowidth": true },
            {
                data: 'id',
                "render": function (data, type, full, meta) {
                    return `<div class="w-75 btn-group" role="group">
                                    <button type="button" data-id="${data}" data-name="${full.name}" data-description="${full.description}" data-validity="${full.validity}"  data-personid="${full.personId}" class="btn-shadow btn btn-info" data-bs-toggle="modal" data-bs-target="#updateModal"> <i class="lnr-pencil"></i> Edit</button>
                                    <a href="javascript:void(0);" onClick="Delete('/Admin/Contract/Delete/${data}')" class="btn-shadow btn btn-danger mx-3"> <i class="lnr-trash"></i> Delete</a>
                                </div>`;
                },
                width: "25%", className: "text-center", orderable: false
            }
        ]
    });
}

$('#nameSearch').on('keyup change', function () {
    dataTable.column(0).search(this.value).draw(); // 0 is the index for the "Name" column
});

// Filter by Description column
$('#descriptionSearch').on('keyup change', function () {
    dataTable.column(1).search(this.value).draw();
});

// New event handler for min and max quantity filtering
$('#minQuantity, #maxQuantity').on('keyup change', function () {
    dataTable.draw();
});

// Call loadDataTable() to initialize your DataTable when the page loads

$('#updateModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var id = button.data('id');
    var name = button.data('name');
    var description = button.data('description');
    var validity = button.data('validity');
    var personId = button.data('personid');
    var modal = $(this);

    modal.find('.modal-body #id').val(id);
    modal.find('.modal-body #name').val(name);
    modal.find('.modal-body #description').val(description);
    modal.find('.modal-body #validity').val(validity);
    modal.find('.modal-body #PersonId').val(personId).trigger('change');

});

