const link = document.querySelectorAll(".add-to-basket");
const div = document.querySelectorAll(".basket-box");
link.forEach(btn => {
    btn.addEventListener("click", function (e) {
        e.preventDefault();
        var endPoint = btn.getAttribute("href");

        Console.log(endPoint);
        //fetch(endPoint)
        //    .then(response => response.Text())
        //    .then(data => {
        //        div.innerHTML = data;
        //})
    })
})