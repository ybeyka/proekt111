using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Contact
{
    public int ID { get; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public List<ContactChange> Changes { get; } = new List<ContactChange>();

    public Contact(int id, string name, string phoneNumber, string email, string address)
    {
        ID = id;
        Name = name;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
    }

    public void AddChange(string fieldName, string oldValue, string newValue)
    {
        var change = new ContactChange(fieldName, oldValue, newValue);
        Changes.Add(change);
    }

    public override string ToString()
    {
        return $"{ID}: {Name}, {PhoneNumber}, {Email}, {Address}";
    }
}

public class ContactChange
{
    public string FieldName { get; }
    public string OldValue { get; }
    public string NewValue { get; }
    public DateTime Timestamp { get; }

    public ContactChange(string fieldName, string oldValue, string newValue)
    {
        FieldName = fieldName;
        OldValue = oldValue;
        NewValue = newValue;
        Timestamp = DateTime.Now;
    }

    public override string ToString()
    {
        return $"{Timestamp}: {FieldName} - {OldValue} -> {NewValue}";
    }
}

public class ContactsManager
{
    public List<Contact> Contacts { get; } = new List<Contact>();

    public void AddContact(string name, string phoneNumber, string email, string address)
    {
        int nextId = Contacts.Count > 0 ? Contacts.Max(c => c.ID) + 1 : 1;
        var contact = new Contact(nextId, name, phoneNumber, email, address);
        Contacts.Add(contact);
    }

    public void ChangeContact(int id, string name, string phoneNumber, string email, string address)
    {
        var contact = Contacts.FirstOrDefault(c => c.ID == id);
        if (contact != null)
        {
            var oldName = contact.Name;
            var oldPhoneNumber = contact.PhoneNumber;
            var oldEmail = contact.Email;
            var oldAddress = contact.Address;

            contact.Name = name;
            contact.PhoneNumber = phoneNumber;
            contact.Email = email;
            contact.Address = address;

            contact.AddChange("Name", oldName, name);
            contact.AddChange("PhoneNumber", oldPhoneNumber, phoneNumber);
            contact.AddChange("Email", oldEmail, email);
            contact.AddChange("Address", oldAddress, address);
        }
        else
        {
            Console.WriteLine("Контакт не знайдено.");
        }
    }

    public void DisplayContacts()
    {
        if (Contacts.Count == 0)
        {
            Console.WriteLine("Немає жодних контактів.");
        }
        else
        {
            foreach (var contact in Contacts)
            {
                Console.WriteLine(contact);
            }
        }
    }

    public void DisplayContactChanges(int id)
    {
        var contact = Contacts.FirstOrDefault(c => c.ID == id);
        if (contact != null)
        {
            Console.WriteLine($"Історія змін для контакта {contact.Name}:");
            foreach (var change in contact.Changes)
            {
                Console.WriteLine(change);
            }
        }
        else
        {
            Console.WriteLine("Контакт не знайдено.");
        }
    }

    public void SaveContactsToFile(string fileName)
    {
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            foreach (var contact in Contacts)
            {
                writer.WriteLine($"{contact.ID},{contact.Name},{contact.PhoneNumber},{contact.Email},{contact.Address}");
                foreach (var change in contact.Changes)
                {
                    writer.WriteLine($"CHANGE,{contact.ID},{change.FieldName},{change.OldValue},{change.NewValue},{change.Timestamp}");
                }
            }
        }

        Console.WriteLine("Контакти та історію змін збережено в файлі.");
    }

    public void LoadContactsFromFile(string fileName)
    {
        if (File.Exists(fileName))
        {
            Contacts.Clear();
            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 5)
                    {
                        int id = int.Parse(parts[0]);
                        string name = parts[1];
                        string phoneNumber = parts[2];
                        string email = parts[3];
                        string address = parts[4];

                        var contact = new Contact(id, name, phoneNumber, email, address);
                        Contacts.Add(contact);
                    }
                }
            }

            Console.WriteLine("Контакти завантажено з файлу.");
        }
        else
        {
            Console.WriteLine("Файл не знайдено.");
        }
    }
}


    class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        ContactsManager contactsManager = new ContactsManager();

        while (true)
        {
            Console.WriteLine("Меню:");
            Console.WriteLine("1. Додати контакт");
            Console.WriteLine("2. Змінити контакт");
            Console.WriteLine("3. Відобразити контакти");
            Console.WriteLine("4. Відобразити історію змін для контакта");
            Console.WriteLine("5. Зберегти контакти в файл");
            Console.WriteLine("6. Завантажити контакти з файлу");
            Console.WriteLine("7. Вихід");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Ім'я: ");
                    string name = Console.ReadLine();
                    Console.Write("Номер телефону: ");
                    string phoneNumber = Console.ReadLine();
                    Console.Write("Електронна пошта: ");
                    string email = Console.ReadLine();
                    Console.Write("Адреса: ");
                    string address = Console.ReadLine();

                    contactsManager.AddContact(name, phoneNumber, email, address);
                    break;

                case "2":
                    Console.Write("ID контакта для зміни: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    {
                        Console.Write("Ім'я: ");
                        string newName = Console.ReadLine();
                        Console.Write("Номер телефону: ");
                        string newPhoneNumber = Console.ReadLine();
                        Console.Write("Електронна пошта: ");
                        string newEmail = Console.ReadLine();
                        Console.Write("Адреса: ");
                        string newAddress = Console.ReadLine();

                        contactsManager.ChangeContact(id, newName, newPhoneNumber, newEmail, newAddress);
                    }
                    else
                    {
                        Console.WriteLine("Некоректний ID контакта.");
                    }
                    break;

                case "3":
                    contactsManager.DisplayContacts();
                    break;

                case "4":
                    Console.Write("ID контакта для перегляду історії: ");
                    if (int.TryParse(Console.ReadLine(), out int historyId))
                    {
                        contactsManager.DisplayContactChanges(historyId);
                    }
                    else
                    {
                        Console.WriteLine("Некоректний ID контакта.");
                    }
                    break;

                case "5":
                    Console.Write("Введіть ім'я файлу для збереження контактів: ");
                    string saveFileName = Console.ReadLine();
                    contactsManager.SaveContactsToFile(saveFileName);
                    break;

                case "6":
                    Console.Write("Введіть ім'я користувача до якого включни завантажити книгу: ");
                    string loadFileName = Console.ReadLine();
                    contactsManager.LoadContactsFromFile(loadFileName);
                    break;

                case "7":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Невідома опція.");
                    break;
            }
        }
    }
}
