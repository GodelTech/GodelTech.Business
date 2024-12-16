# GodelTech.Business

# Description
GodelTech.Business is a .NET library designed for implementing services in the business layer of applications. It provides interfaces and base classes that support common operations such as listing, retrieving, adding, editing, and deleting entities. This library aims to simplify the development of business services by offering reusable components and reducing boilerplate code.

Using `IBusinessService<TDto, in TAddDto, in TEditDto, in TKey>` and `BusinessService<TEntity, TKey, TUnitOfWork, TDto, TAddDto, TEditDto>` you will get main methods e.g. GetList, Get, Add, Edit, Delete using [GodelTech.Data](https://github.com/GodelTech/GodelTech.Data) project.

# License
This project is licensed under the MIT License. See the LICENSE file for more details.