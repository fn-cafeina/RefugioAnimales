using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Refugio.App.Data;
using Refugio.App.Models;
using Refugio.App.Services;

namespace Refugio.App.ViewModels;

public partial class UsuariosViewModel : ViewModelBase
{
    private readonly UsuarioRepository _repo = new();
    private int? _editingId;
    private string _editingHash = "";

    public ObservableCollection<Usuario> Usuarios { get; } = new();
    public IReadOnlyList<string> Roles { get; } = new[] { "voluntario", "veterinario", "admin" };

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private Usuario? selectedUsuario;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    private string editNombre = "";

    [ObservableProperty]
    private string editEmail = "";

    [ObservableProperty]
    private string editRol = "voluntario";

    [ObservableProperty]
    private bool editActivo = true;

    [ObservableProperty]
    private string editPassword = "";

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private string? infoMessage;

    public bool HasSelection => SelectedUsuario is not null;

    public UsuariosViewModel() => Load();

    [RelayCommand]
    private void Load()
    {
        try
        {
            ErrorMessage = null;
            var lista = _repo.GetAll();
            Usuarios.Clear();
            foreach (var usuario in lista) Usuarios.Add(usuario);
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudieron cargar los usuarios: " + ex.Message;
        }
    }

    [RelayCommand]
    private void New()
    {
        _editingId = null;
        _editingHash = "";
        SelectedUsuario = null;
        IsEditing = true;
        ErrorMessage = null;
        InfoMessage = null;
        EditNombre = "";
        EditEmail = "";
        EditRol = "voluntario";
        EditActivo = true;
        EditPassword = "";
    }

    [RelayCommand]
    private void EditSelected()
    {
        if (SelectedUsuario is null) return;

        _editingId = SelectedUsuario.Id;
        _editingHash = SelectedUsuario.PasswordHash;
        IsEditing = true;
        ErrorMessage = null;
        InfoMessage = null;
        EditNombre = SelectedUsuario.Nombre;
        EditEmail = SelectedUsuario.Email;
        EditRol = SelectedUsuario.Rol;
        EditActivo = SelectedUsuario.Activo;
        EditPassword = "";
    }

    [RelayCommand]
    private void Save()
    {
        ErrorMessage = null;
        InfoMessage = null;

        if (string.IsNullOrWhiteSpace(EditNombre) || string.IsNullOrWhiteSpace(EditEmail))
        {
            ErrorMessage = "Nombre y email son obligatorios.";
            return;
        }

        if (_editingId is null && string.IsNullOrWhiteSpace(EditPassword))
        {
            ErrorMessage = "La contraseña es obligatoria para un usuario nuevo.";
            return;
        }

        if (_editingId is not null && !EditActivo && _editingId == Session.CurrentUser?.Id)
        {
            ErrorMessage = "No podés desactivar tu propio usuario.";
            return;
        }

        try
        {
            var usuario = new Usuario
            {
                Id = _editingId ?? 0,
                Nombre = EditNombre.Trim(),
                Email = EditEmail.Trim().ToLowerInvariant(),
                Rol = EditRol,
                Activo = EditActivo,
                PasswordHash = string.IsNullOrWhiteSpace(EditPassword)
                    ? _editingHash
                    : PasswordHasher.Hash(EditPassword)
            };

            if (_editingId is null)
            {
                _repo.Insert(usuario);
                InfoMessage = "Usuario creado correctamente.";
            }
            else
            {
                _repo.Update(usuario);
                InfoMessage = "Usuario actualizado correctamente.";
            }

            IsEditing = false;
            _editingId = null;
            _editingHash = "";
            EditPassword = "";
            Load();
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudo guardar el usuario: " + ex.Message;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        IsEditing = false;
        _editingId = null;
        _editingHash = "";
        EditPassword = "";
        ErrorMessage = null;
    }
}
