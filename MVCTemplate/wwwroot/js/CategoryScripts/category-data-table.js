$(document).ready(function () {
    loadDataTableCategory();
});

// Step 1: Define the Delete function
function Delete(url) {
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
}

// Step 2: Define the DataTable and rendering logic

function loadDataTableCategory() {
    dataTableCategory = $('#categoryTable').DataTable({
        "ajax": { url: '/Admin/Category/GetAllCategory' },
        "columns": [
            { data: 'idCategory', "autowidth": true },
            { data: 'nameCategory', "autowidth": true },
            { data: 'codeCategory', "autowidth": true },
            {
                data: 'idCategory',
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
    var idCategory = button.data('idCategory');
    var nameCategory = button.data('nameCategory');
    var codeCategory = button.data('codeCategory');
    var modal = $(this);

    modal.find('.modal-body #productId').val(idCategory);
    modal.find('.modal-body #name').val(nameCategory);
    modal.find('.modal-body #code').val(codeCategory);
});

// duplicate of product datatables