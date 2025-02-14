# Sanko Hospital

**Sanko Hospital** is a web-based hospital management system built using ASP.NET MVC. The application is designed to manage various aspects of hospital operations through different user roles. The system provides dedicated dashboards, profile management, and settings pages for each role.

## Features

### User Roles

The application supports the following five roles:

- **User:**  
  - Initially, every user is created with the "User" role.  
  - A user waits for an administrator to assign them a specific role.

- **Admin:**  
  - Admins have full access to all system features.  
  - They can assign roles to other users, view overall statistics, and manage user accounts.

- **Receptionist:**  
  - The receptionist can view room statuses and occupancy.  
  - They have the ability to add new patients and edit patient records.
  
- **Nurse:**  
  - Nurses can view a list of patients.  
  - They can check the daily status of patients (e.g., whether they have been attended to) and manage patient care.

- **Cleaner:**  
  - Cleaners can access room details.  
  - They are responsible for monitoring and updating the cleaning status of the rooms.

## Dashboard, Profile, and Settings

Each user role has its own dashboard, profile, and settings pages, though the structure is consistent across roles:

- **Dashboard:**  
  Displays relevant statistics and key performance indicators. For example, Admins can see overall user counts and role distributions, while other roles see information pertinent to their responsibilities.

- **Profile:**  
  Each user can view and update their profile information, including a default or role-specific avatar, username, and role.

- **Settings:**  
  Settings pages allow users to update their username and password, as well as delete their account. These actions prompt confirmation via toast notifications to prevent accidental changes.

## How It Works

1. **User Registration and Role Assignment:**
   - All new users are initially registered as "User".
   - An admin assigns additional roles (such as Receptionist, Nurse, or Cleaner) based on the needs of the hospital.

2. **Role-Specific Dashboards:**
   - **Admin Dashboard:** Provides overall system statistics (e.g., total users, role counts) and management features such as inline role updates and user deletion.
   - **Receptionist Dashboard:** Displays room statuses and patient entry forms to manage patient admissions and edits.
   - **Nurse Dashboard:** Shows a patient list and allows nurses to mark whether a patient has been checked for the day.
   - **Cleaner Dashboard:** Lists room details along with cleaning status, enabling cleaners to update room conditions.

3. **Profile and Settings:**
   - All roles have access to a consistent profile page where they can view and edit their information.
   - The settings page is common across roles, allowing users to change their username, update their password, or delete their account. Actions are confirmed with toast messages for extra safety.

## Technologies Used

- **Backend:** ASP.NET MVC, C#
- **Frontend:** HTML, CSS, JavaScript (jQuery)
- **Authentication:** JWT (JSON Web Tokens)
- **Data Access:** NHibernate
- **Other:** Custom toast notifications for confirmation dialogs

## Installation

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/ozguncanbey/SankoHospital.git
   cd SankoHospital
   ```

2. **Configure the Database:**
   Update the connection string in the `appsettings.json` file.

3. **Configure API Settings:**
   Set the `ApiSettings:BaseUrl` in `appsettings.json` to point to your API endpoint.

4. **Run the Application:**
   Build and run the application using Visual Studio or the .NET CLI:
   ```bash
   dotnet run
   ```

## Usage

- **Login:**  
  Users log in and initially have the "User" role. An admin will assign the appropriate role later.

- **Role Assignment (Admin):**  
  The admin uses the dashboard to view user statistics and assign roles to users.

- **Patient Management (Receptionist):**  
  Receptionists can manage patient admissions and edits through their dedicated dashboard.

- **Patient Monitoring (Nurse):**  
  Nurses check the daily status of patients from their dashboard.

- **Room Cleaning (Cleaner):**  
  Cleaners update the cleaning status of hospital rooms from their dashboard.

## License

This project is licensed under the [MIT License](LICENSE).

---
