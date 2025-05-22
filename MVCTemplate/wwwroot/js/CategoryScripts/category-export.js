
document.querySelector("#button-excel").addEventListener("click", async function () {
    var table = $('#Categorys').DataTable(); // ??change to account for the name in ApplicationDbContext
    var searchValue = table.search();
    var dataToExport;

    if (searchValue) {
        dataToExport = table.rows({ search: 'applied' }).data().toArray();
    } else {
        let response = await fetch('/Admin/Category/GetAllCategory'); //change to account for url naming
        let result = await response.json();
        dataToExport = result.data;
    }

    dataToExport.sort((a, b) => a.nameCategory.localeCompare(b.nameCategory)); // ^ change to account for name of data (Model)

    let tempTable = document.createElement('table');
    let thead = document.createElement('thead');
    thead.innerHTML = '<tr><th>ID</th> <th>Name</th> <th>Code</th></tr>';
    tempTable.appendChild(thead);

    let tbody = document.createElement('tbody');
    dataToExport.forEach(row => {
        let tr = document.createElement('tr');
        tr.innerHTML = `<td>${row.idCategory}</td> <td>${row.nameCategory}</td> <td>${row.codeCategory}</td>`;
        tbody.appendChild(tr); // ^ change to account for name of data (Model) and number of column 
    });
    tempTable.appendChild(tbody);

    TableToExcel.convert(tempTable, { name: "Category.xlsx" });
});