const CV_FIELDS = [
    "fullName",
    "languages",
    "skills",
    "email",
    "phone",
    "education",
    "additionalInfo"
];

function send() {
    const payload = {};
    CV_FIELDS.forEach(id => {
        payload[id] = document.getElementById(id).value;
    });

    fetch("/api/submit", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(payload)
    })
    .then(res => res.text())
    .then(data => {
        document.getElementById("result").innerText = data;
    })
    .catch(err => {
        document.getElementById("result").innerText = "Error!";
        console.error(err);
    });
}
