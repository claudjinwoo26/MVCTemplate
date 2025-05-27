
//<script src="https://cdn.jsdelivr.net/npm/exceljs/dist/exceljs.min.js"></script> this has to be put in _layout

document.querySelector("#button-excel").addEventListener("click", async function () {
    var table = $('#Packages').DataTable();
    var searchValue = table.search();
    var dataToExport;

    if (searchValue) {
        dataToExport = table.rows({ search: 'applied' }).data().toArray();
    } else {
        let response = await fetch('/Admin/Package/GetAllPackages');
        let result = await response.json();
        dataToExport = result.data;
    }

    dataToExport.sort((a, b) => a.name.localeCompare(b.name));

    const workbook = new ExcelJS.Workbook();
    const worksheet = workbook.addWorksheet('Packages');

    // Add columns
    worksheet.columns = [
        { header: 'ID', key: 'id'},
        { header: 'Name', key: 'name'},
        { header: 'Description', key: 'description'},
        { header: 'Priority', key: 'priority'}
    ];

    // Add rows
    dataToExport.forEach(row => {
        worksheet.addRow(row);
    });

    // Add autofilter on header row (first row)
    worksheet.autoFilter = {
        from: 'A1',
        to: 'D1',
    };

    // Generate buffer and trigger download
    const buffer = await workbook.xlsx.writeBuffer();

    // Create blob and trigger download
    const blob = new Blob([buffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'Package.xlsx';
    a.click();
    URL.revokeObjectURL(url);
});


/*
document.querySelector("#button-excel").addEventListener("click", async function () {
    var table = $('#Packages').DataTable(); // ??change to account for the name in ApplicationDbContext
    var searchValue = table.search();
    var dataToExport;

    if (searchValue) {
        dataToExport = table.rows({ search: 'applied' }).data().toArray();
    } else {
        let response = await fetch('/Admin/Package/GetAllPackages'); //change to account for url naming
        let result = await response.json();
        dataToExport = result.data;
    }

    dataToExport.sort((a, b) => a.name.localeCompare(b.name)); // ^ change to account for name of data (Model)

    let tempTable = document.createElement('table');
    let thead = document.createElement('thead');
    thead.innerHTML = '<tr><th>ID</th> <th>Name</th> <th>Description</th> <th>Priority</th> </tr>';
    tempTable.appendChild(thead);

    let tbody = document.createElement('tbody');
    dataToExport.forEach(row => {
        let tr = document.createElement('tr');
        tr.innerHTML = `<td>${row.id}</td> <td>${row.name}</td> <td>${row.description}</td> <td>${row.priority}</td>`;
        tbody.appendChild(tr); // ^ change to account for name of data (Model) and number of column 
    });
    tempTable.appendChild(tbody);

    TableToExcel.convert(tempTable, { name: "Package.xlsx" });
});*/