$(document).ready(function () {
    // Initialize Ladda buttons
    Ladda.bind('.ladda-button');
    //Initialize Create Button
    const createAddFormBtn = $("#createAddFormSubmitBtn").get(0);
    const createAddFormLadda = Ladda.create(createAddFormBtn);
    $('#createForm').submit(async function (event) {
        event.preventDefault();
        var formData = new FormData(this);
        clearErrors(event.currentTarget);
        try {
            const response = await $.ajax({
                url: $(this).attr('action'),
                type: $(this).attr('method'),
                data: formData,
                contentType: false,
                processData: false,
            })
            //Show Alert
            toastr.success(response.message);
            //Refresh Data Table
            dataTable.ajax.reload();
            //Reset Form
            $('#createForm')[0].reset();
            //Hide Modal
            $('#createModal').modal('hide');
        } catch (error) {
            const formErrors = error?.responseJSON?.errors ?? {}
            showErrors(formErrors, event.currentTarget)
            toastr.error(error?.responseJSON?.message);
        } finally {
            createAddFormLadda.stop()
        }
    });
    //Initiliaze Update Button
    const createEditFormBtn = $("#createEditFormSubmitBtn").get(0);
    const createEditFormLadda = Ladda.create(createEditFormBtn);

    // Handle update form submission
    $('#updateForm').submit(async function (event) {
        event.preventDefault();
        var formData = new FormData(this);
        clearErrors(event.currentTarget);
        try {
            const response = await $.ajax({
                url: $(this).attr('action'),
                type: $(this).attr('method'),
                data: formData,
                contentType: false,
                processData: false,
            });
            //Show Alert
            toastr.success(response.message);
            //Refresh Data Table
            dataTable.ajax.reload();
            //Reset Form
            $('#updateForm')[0].reset();
            //Hide Modal
            $('#updateModal').modal('hide');
        } catch (error) {
            const formErrors = error?.responseJSON?.errors ?? {}
            showErrors(formErrors, event.currentTarget)
            toastr.error(error?.responseJSON?.message);
        } finally {
            createEditFormLadda.stop();
        }
    });
    /*

    $('#deleteForm').submit(function (e) {
        e.preventDefault(); // Prevent the default form submission

        var form = $(this);
        var itemId = $('#itemId').val(); // Get the item ID from the hidden input

        // Send an AJAX request to delete the item
        $.ajax({
            url: form.attr('action'), // Form action URL (delete URL)
            type: 'POST',
            data: form.serialize(), // Serialize the form data
            success: function (response) {
                // Success: Hide the modal
                $('#deleteModal').modal('hide');

                // Optionally: Remove the deleted item from the list (if you're not reloading the page)
                $('#itemRow' + itemId).remove(); // Assumes you have an ID or class for the item row

                // Show success message or update UI as needed
                alert('Item deleted successfully!');
            },
            error: function (xhr, status, error) {
                // Error: Handle any errors during the AJAX request
                alert('There was an error deleting the item. Please try again.');
            }
        });
    });
    */
});