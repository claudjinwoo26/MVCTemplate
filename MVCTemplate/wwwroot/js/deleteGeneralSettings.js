async function Delete(url) {
    const result = await Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    });

    if (result.isConfirmed) {
        try {
            const response = await $.ajax({
                url: url,
                type: 'DELETE',
            });
            dataTable.ajax.reload();
            toastr.success(response.message);
        } catch (error) {
            toastr.error(error?.responseJSON?.message);
        }
    }
}