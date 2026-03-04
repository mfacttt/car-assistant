function openCarModal() {
    document.getElementById("carModal").classList.add("active");
}

function openNotificationModal() {
    const modal = document.getElementById("notificationModal");
    const typeInput = document.getElementById("notifType");
    const dateInput = document.getElementById("notifDate");
    const title = document.getElementById("notificationModalTitle");

    if (typeInput) typeInput.value = "Техосмотр";
    if (dateInput) {
        const today = new Date().toISOString().slice(0, 10);
        dateInput.value = today;
    }
    if (title) title.textContent = "Новое событие";

    modal.classList.add("active");
}

function closeModal(type) {
    if (type === "car") {
        document.getElementById("carModal").classList.remove("active");
    } else if (type === "notification") {
        document.getElementById("notificationModal").classList.remove("active");
    }
}

function saveCar() {
    alert("Данные автомобиля сохранены (демо)");
    closeModal("car");
}

let currentItem = null;
let startX = 0;
let currentX = 0;
let isSwiping = false;
const swipeThreshold = 40;

function startSwipe(event) {
    const clientX = event.type.startsWith("touch") ? event.touches[0].clientX : event.clientX;
    const item = event.target.closest(".notification-item");
    if (!item) return;

    document.querySelectorAll(".notification-item.swiped").forEach(element => {
        element.classList.remove("swiped");
    });

    currentItem = item;
    startX = clientX;
    isSwiping = true;
}

function swipeMove(event) {
    if (!isSwiping || !currentItem) return;

    const clientX = event.type.startsWith("touch") ? event.touches[0].clientX : event.clientX;
    const deltaX = clientX - startX;

    if (deltaX < 0 && Math.abs(deltaX) > 10) {
        event.preventDefault();
        const translateX = Math.max(-60, deltaX);
        currentItem.style.transform = `translateX(${translateX}px)`;
        currentX = translateX;
    }
}

function endSwipe() {
    if (!isSwiping || !currentItem) {
        resetSwipe();
        return;
    }

    if (currentX < -swipeThreshold) {
        currentItem.classList.add("swiped");
        currentItem.style.transform = "";

        const container = currentItem.closest(".notification-container");
        if (container) {
            const deleteBtn = container.querySelector(".delete-btn");
            if (deleteBtn) {
                deleteBtn.classList.add("visible");
            }
        }
    } else {
        currentItem.style.transform = "";
    }

    resetSwipe();
}

function cancelSwipe() {
    if (currentItem) {
        currentItem.style.transform = "";
    }

    resetSwipe();
}

function resetSwipe() {
    currentItem = null;
    startX = 0;
    currentX = 0;
    isSwiping = false;
}

document.addEventListener("click", event => {
    if (!event.target.closest(".notification-container")) {
        document.querySelectorAll(".notification-item.swiped").forEach(element => {
            element.classList.remove("swiped");
        });

        document.querySelectorAll(".delete-btn.visible").forEach(element => {
            element.classList.remove("visible");
        });
    }
});
