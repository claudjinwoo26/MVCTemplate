// Date range filter using DataTables custom filter extension
$.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
    let min = $('#startDate').val();
    let max = $('#endDate').val();
    let dateStr = data[2]; // "createdAt" is column index 3

    if (!dateStr) return false;

    let createdDate = new Date(dateStr);
    createdDate.setHours(0, 0, 0, 0); // Normalize time to 00:00 for consistency

    let minDate = min ? new Date(min) : null;
    let maxDate = max ? new Date(max) : null;

    if (minDate) minDate.setHours(0, 0, 0, 0);
    if (maxDate) maxDate.setHours(23, 59, 59, 999); // Include full end date

    if (
        (!minDate || createdDate >= minDate) &&
        (!maxDate || createdDate <= maxDate)
    ) {
        return true;
    }

    return false;
});

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
                    return date.toLocaleDateString('en-US', options);
                },
                "autowidth": true
            },
            { data: 'personId', "autowidth": true },
            {
                data: 'id',
                "render": function (data, type, full, meta) {
                    return `<div class="w-75 btn-group" role="group">
            <button type="button"
                data-id="${data}"
                data-name="${full.name}"
                data-description="${full.description}"
                data-validity="${full.validity}"
                data-person-id="${full.personId}"
                class="btn-shadow btn btn-info"
                data-bs-toggle="modal"
                data-bs-target="#updateModal">
                <i class="lnr-pencil"></i> Edit
            </button>
            <a href="javascript:void(0);" onClick="Delete('/Admin/Contract/Delete/${data}')" class="btn-shadow btn btn-danger mx-3">
                <i class="lnr-trash"></i> Delete
            </a>
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

$('#startDate, #endDate').on('change', function () {
    dataTable.draw();
});

$('#updateModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var id = button.data('id');
    var name = button.data('name');
    var description = button.data('description');
    var validity = button.data('validity');
    var personId = button.data('personId');
    var modal = $(this);

    modal.find('#Contract_Id').val(id);
    modal.find('#Contract_Name').val(name);
    modal.find('#Contract_Description').val(description);
    modal.find('#Contract_Validity').val(validity ? validity.split('T')[0] : '');

    const personSelect = modal.find('#Contract_PersonId');

    // Check if the current person is already in the dropdown
    if (personSelect.find(`option[value="${personId}"]`).length === 0 && personId) {
        // Fetch the person name via AJAX
        $.get(`/Admin/Contract/GetPersonNameById?id=${personId}`, function (response) {
            personSelect.append(new Option(response.name, personId, true, true)).trigger('change');
        }).fail(function () {
            // If fetch fails, fallback to ID
            personSelect.append(new Option(`(Unknown Person #${personId})`, personId, true, true)).trigger('change');
        });
    } else {
        personSelect.val(personId).trigger('change');
    }

});


