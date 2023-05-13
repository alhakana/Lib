"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/libraryHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start()
    .then(() => console.log("Connection started"))
    .catch(err => console.error(err.toString()));

connection.on("CreateReservation", (reservation) => {
    console.log("Received a CreateReservation message", reservation);

    // Select the table body
    const tableBody = document.querySelector('.table tbody');

    // Create a new row
    const row = document.createElement('tr');

    // Create and append new cells to the row
    const titleCell = document.createElement('td');
    titleCell.textContent = reservation.bookTitle;
    row.appendChild(titleCell);

    const authorCell = document.createElement('td');
    authorCell.textContent = reservation.bookAuthor;
    row.appendChild(authorCell);

    const userCell = document.createElement('td');
    userCell.textContent = reservation.userName;
    row.appendChild(userCell);

    const reservationDateCell = document.createElement('td');
    const reservationDate = new Date(reservation.reservationDate);
    const formattedDate = `${reservationDate.getDate()}.${reservationDate.getMonth() + 1}.${reservationDate.getFullYear()}.`;
    reservationDateCell.textContent = formattedDate;
    row.appendChild(reservationDateCell);

    const dueDateCell = document.createElement('td');
    if (new Date(reservation.dueDate) <= new Date('0001-01-02T00:00:00')) {
        dueDateCell.textContent = "Not set";
    } else {
        dueDateCell.textContent = new Date(reservation.dueDate).toDateString();
    }
    row.appendChild(dueDateCell);

    const statusCell = document.createElement('td');
    statusCell.textContent = reservation.isReturned ? "Returned" : (reservation.status == 0 ? "Active" : "Pending Approval");
    statusCell.id = "reservation-status-" + reservation.id;
    row.appendChild(statusCell);

    // Add the Actions cell
    const actionsCell = document.createElement('td');
    if (!reservation.isReturned) {
        if (reservation.status == 1) {
            // Approve button
            const approveForm = document.createElement('form');
            approveForm.method = 'post';
            approveForm.style.display = 'inline';
            approveForm.action = '/Reservation/ApproveReservation';

            // Create the anti-forgery token input
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

            const tokenInput = document.createElement('input');
            tokenInput.type = 'hidden';
            tokenInput.name = '__RequestVerificationToken';
            tokenInput.value = token;
            approveForm.appendChild(tokenInput);

            const approveInput = document.createElement('input');
            approveInput.type = 'hidden';
            approveInput.name = 'reservationId';
            approveInput.value = reservation.id;
            approveForm.appendChild(approveInput);

            const approveButton = document.createElement('button');
            approveButton.type = 'submit';
            approveButton.className = 'btn btn-sm btn-primary';
            approveButton.textContent = 'Approve';
            approveForm.appendChild(approveButton);

            actionsCell.appendChild(approveForm);

            // Reject button
            const rejectForm = document.createElement('form');
            rejectForm.method = 'post';
            rejectForm.style.display = 'inline';
            rejectForm.action = '/Reservation/RejectReservation';

            // Create the anti-forgery token input for the reject form
            const rejectTokenInput = document.createElement('input');
            rejectTokenInput.type = 'hidden';
            rejectTokenInput.name = '__RequestVerificationToken';
            rejectTokenInput.value = token; // Use the same token value obtained before
            rejectForm.appendChild(rejectTokenInput);

            const rejectInput = document.createElement('input');
            rejectInput.type = 'hidden';
            rejectInput.name = 'reservationId';
            rejectInput.value = reservation.id;
            rejectForm.appendChild(rejectInput);

            const rejectButton = document.createElement('button');
            rejectButton.type = 'submit';
            rejectButton.className = 'btn btn-sm btn-danger';
            rejectButton.textContent = 'Reject';
            rejectForm.appendChild(rejectButton);

            actionsCell.appendChild(rejectForm);
        } else {
            const returnForm = document.createElement('form');
            returnForm.method = 'post';
            returnForm.style.display = 'inline';
            returnForm.action = '/Reservation/ReturnBook';

            const returnInput = document.createElement('input');
            returnInput.type = 'hidden';
            returnInput.name = 'reservationId';
            returnInput.value = reservation.id;
            returnForm.appendChild(returnInput);

            const returnButton = document.createElement('button');
            returnButton.type = 'submit';
            returnButton.className = 'btn btn-sm btn-success';
            returnButton.textContent = 'Return';
            returnButton.id = 'return-button-' + reservation.id;
            returnForm.appendChild(returnButton);

            actionsCell.appendChild(returnForm);
        }
    }
    row.appendChild(actionsCell);

    // Append the new row to the table body
    tableBody.appendChild(row);


});

connection.on("ApproveReservation", (reservation) => {
    console.log("Received a ApproveReservation message", reservation);

    // Get the row for the approved reservation
    const row = document.querySelector(`tr[data-id="${reservation.id}"]`);

    if (row) {
        // Find and update the due date cell
        const dueDateCell = row.children[3]; 
        const dueDate = new Date(reservation.dueDate);
        const formattedDate = `${dueDate.getDate()}.${dueDate.getMonth() + 1}.${dueDate.getFullYear()}.`;
        dueDateCell.textContent = formattedDate;

        // Find and update the return status cell
        const statusCell = row.children[4]; 
        statusCell.textContent = "Active";

        // Find and disable the cancel button
        const cancelButton = row.querySelector("button");
        cancelButton.disabled = true;
        cancelButton.classList.remove("btn-danger");
        cancelButton.classList.add("btn-secondary");
    }
});

connection.on("RejectReservation", (reservationId) => {
    console.log("Received a RejectReservation message", reservationId);

    // Get the row for the rejected reservation
    const row = document.querySelector(`tr[data-id="${reservationId}"]`);

    if (row) {
        // Find and update the status cell
        const statusCell = row.children[4]; // Assuming the status is the 5th cell
        statusCell.textContent = "Rejected";

        // Find and disable the cancel button
        const cancelButton = row.querySelector("button");
        cancelButton.disabled = true;
        cancelButton.classList.remove("btn-danger");
        cancelButton.classList.add("btn-secondary");
    }
});