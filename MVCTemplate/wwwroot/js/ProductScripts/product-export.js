/*
document.addEventListener('DOMContentLoaded', function () {
    // Add event listener to the "Export" button
    document.getElementById('button-excel').addEventListener('click', function () {
        // Create a form dynamically to submit via POST
        var form = document.createElement('form');
        form.method = 'POST';
        form.action = '/Admin/Product/ExportToExcel'; // Adjust the action URL as needed

        // Submit the form
        document.body.appendChild(form);
        form.submit();
    });
}); NOT WORKING!!
*/

document.querySelector("#button-excel").addEventListener("click", async function () {
    var table = $('#Products').DataTable(); // change to account for the name of the class (Model)
    var searchValue = table.search();
    var dataToExport;

    if (searchValue) {
        dataToExport = table.rows({ search: 'applied' }).data().toArray();
    } else {
        let response = await fetch('/Admin/Product/GetAllProducts'); //change to account for url naming
        let result = await response.json();
        dataToExport = result.data;
    }

    dataToExport.sort((a, b) => a.name.localeCompare(b.name));

    let tempTable = document.createElement('table');
    let thead = document.createElement('thead');
    thead.innerHTML = '<tr><th>ID</th> <th>Name</th> <th>Description</th> <th>Quantity</th> </tr>'; 
    tempTable.appendChild(thead); // ^ change to account for name of data (Model) and number of column 

    let tbody = document.createElement('tbody');
    dataToExport.forEach(row => {
        let tr = document.createElement('tr');
        tr.innerHTML = `<td>${row.id}</td> <td>${row.name}</td> <td>${row.description}</td> <td>${row.quantity}</td>`;
        tbody.appendChild(tr); // ^ change to account for name of data (Model) and number of column 
    });
    tempTable.appendChild(tbody);

    TableToExcel.convert(tempTable, { name: "Product.xlsx" });
});
