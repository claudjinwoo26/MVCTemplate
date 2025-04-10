const clearErrors = (form = null) => {
    if (form === null) {
        console.warn("form is null")
        return;
    }
    if (typeof form === "string") {
        form = document.querySelector(form);
    }
    const errorPlaceholders = form.querySelectorAll("span[data-valmsg-for]")
    errorPlaceholders?.forEach(e => {
        e.innerText = "";
        e.style.display = "none";
    })

}


const showErrors = (errors = {}, form = null) => {
    if (!errors) {
        throw ("Errors object cannot be null or undefined")
    }
    if (!form) {
        throw ("Form element is required");
    }

    for (const [key, value] of Object.entries(errors)) {

        const message = value?.[0];
        if (!message) {
            console.warn(`error with name ${key} has no mesage`)
            continue;
        }
        const errorSpan = form.querySelector(`span[data-valmsg-for="${key}"]`);
        if (!errorSpan) {
            console.warn(`error with name ${key} has no error placeholder`)
            continue;
        }
        errorSpan.innerText = message;
        errorSpan.style.display = "block"

    }
}

