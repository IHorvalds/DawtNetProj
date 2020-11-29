# DawtNetProj

Uni project. Cool idea and nice to be given a theme. Makes it easier to make decisions about what's important in the project and what isn't.

### Few first impressions
1. The ORM is deceiving. Entity Framework makes it seem like it will handle operations on its own and you just describe the models. CoreData seems much more user friendly; so was Django iirc.
2. So. Many. Strings. It's so easy to get mixed up and get errors if you're not careful about how you manage your strings. Binds depend on exact matches between variable names. You can declare your own bindings (and probably some sort of mappings too, but I couldn't find them fast enough to be useful for this project).
3. The packages are really cool. I've looked a bit over the available packages. Seems like there's one for everything.
4. Pass more objects to views! Here you can only pass a model and two temporary "bags" or variables. Wouldn't it be easier if you could just pass more objects to the views with an actual name you can track instead of defensively checking if the "bags" also have the variables you're looking for?

Either way, cool framework! Very little work needed to get a functioning site up and running.
