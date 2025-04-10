
var message = localStorage.getItem('toastrMessage');
if (message) {
    toastr.success(message);
    localStorage.removeItem('toastrMessage');
}