$(document).ready(function () {
    loadDataTable();
});

/*
// Step 1: Define the Delete function
function Delete(url) { //function name needs to be consistent with controller
    // Confirm deletion before proceeding
    if (confirm('Are you sure you want to delete this product?')) {
        $.ajax({
            url: url,        // The URL to call (this should include the product ID)
            type: 'DELETE',  // Method to delete the product
            success: function (response) {
                if (response.success) {
                    // Reload the DataTable or remove the deleted row
                    //$('#productTable').DataTable().ajax.reload(); - error
                    dataTable.ajax.reload();
                    alert('Product deleted successfully');
                } else {
                    alert('Error deleting product');
                }
            },
            error: function (xhr, status, error) {
                alert('An error occurred while deleting the product');
            }
        });
    }
}OLD DELETE*/

// Step 2: Define the DataTable and rendering logic

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
                                    <button type="button" data-id="${data}" data-name="${full.name}" data-description="${full.description}" data-quantity="${full.quantity}" class="btn-shadow btn btn-info" data-bs-toggle="modal" data-bs-target="#updateModal"> <i class="lnr-pencil"></i> Edit</button>
                                    <a href="javascript:void(0);" onClick="Delete('/Admin/Product/Delete/${data}')" class="btn-shadow btn btn-danger mx-3"> <i class="lnr-trash"></i> Delete</a>
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
    var id = button.data('id');
    var name = button.data('name');
    var description = button.data('description');
    var quantity = button.data('quantity');
    var modal = $(this);

    modal.find('.modal-body #productId').val(id);
    modal.find('.modal-body #name').val(name);
    modal.find('.modal-body #description').val(description);
    modal.find('.modal-body #quantity').val(quantity);
});

