document.addEventListener("DOMContentLoaded", function () {
    var currentURL = window.location.href.toLowerCase();
    var currentURLArray = currentURL.split("/"); // Split current URL by "/"
    var controller1 = currentURLArray[3];  // Assuming the controller is the fourth part of the URL
    var controller2 = currentURLArray[4];
    var controller = controller1 + "/" + controller2

    document.querySelectorAll(".sideBarLink").forEach((link) => {
        var linkHref = link.href.toLowerCase();
        var linkHrefArray = linkHref.split("/"); // Split link href by "/"
        var linkController1 = linkHrefArray[3]; // Assuming the controller is the fourth part of the link href
        var linkController2 = linkHrefArray[4];
        var linkController = linkController1 + "/" + linkController2
        // Check if the controllers match
        if (linkController === controller) {
            link.classList.add("mm-active");
            link.setAttribute("aria-current", "page");


            const parent = link.closest('.mainMenu');
            if (parent) {
                parent.classList.add("mm-active");
            }
        }
    });
});
